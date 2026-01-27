// FamilyShow Web - Custom Family Tree Layout
// Matches WPF DiagramLogic positioning with generation-aligned rows

class FamilyTreeLayout {
    constructor(familyData) {
        this.familyData = familyData;
        this.nodePositions = new Map();
        this.nodeSpacing = { x: 120, y: 150 };
        this.nodeSize = { width: 80, height: 100 };
        this.rowHeight = 180;
        this.spouseYOffset = -20; // Spouses positioned slightly above their partner
        this.siblingColor = '#FFD700'; // Yellow for siblings
        this.generations = new Map(); // Track generation levels
    }

    /**
     * Calculate layout for a focus person and their extended family
     * Uses generation-based positioning to align all people in same generation
     */
    calculateLayout(focusPersonId) {
        console.log('Calculating custom layout for person:', focusPersonId);

        const person = this.findPerson(focusPersonId);
        if (!person) {
            console.error('Person not found:', focusPersonId);
            return { nodes: [], edges: [] };
        }

        this.nodePositions.clear();
        this.generations.clear();
        const nodes = [];
        const edges = [];

        // Build generation map first
        this.buildGenerationMap(person, 0);

        // Position each generation as a complete row
        this.positionAllGenerations(person, nodes, edges);

        console.log('Layout complete:', nodes.length, 'nodes,', edges.length, 'edges');
        return { nodes, edges };
    }

    /**
     * Build a map of all people organized by generation level
     * Generation 0 = focus person
     * Negative generations = ancestors (parents, grandparents, etc.)
     * Positive generations = descendants (children, grandchildren, etc.)
     * 
     * RULES:
     * - Only siblings of the ORIGINALLY SELECTED focus person are included
     * - For ancestors: Both parents are included (they're both ancestors)
     * - For descendants: ONLY descendants of the focus person (not siblings' descendants)
     */
    buildGenerationMap(focusPerson, focusGeneration) {
        const visited = new Set();

        // Map to track person -> generation level
        const personGeneration = new Map();
        personGeneration.set(focusPerson.Id, focusGeneration);

        // Track the focus person ID - ONLY this person's siblings will be shown
        this.focusPersonId = focusPerson.Id;

        // Add focus person to generation 0
        if (!this.generations.has(focusGeneration)) {
            this.generations.set(focusGeneration, []);
        }

        visited.add(focusPerson.Id);
        this.generations.get(focusGeneration).push(focusPerson);

        // Add focus person's spouse(s) to generation 0
        const focusSpouses = this.getSpouses(focusPerson);
        focusSpouses.forEach(spouse => {
            if (!visited.has(spouse.Id)) {
                visited.add(spouse.Id);
                personGeneration.set(spouse.Id, focusGeneration);
                this.generations.get(focusGeneration).push(spouse);
            }
        });

        // Add focus person's siblings to generation 0 (but NOT their descendants)
        const focusSiblings = this.getSiblings(focusPerson);
        focusSiblings.forEach(sibling => {
            if (!visited.has(sibling.Id)) {
                visited.add(sibling.Id);
                personGeneration.set(sibling.Id, focusGeneration);
                this.generations.get(focusGeneration).push(sibling);

                // Add sibling's spouse(s)
                const siblingSpouses = this.getSpouses(sibling);
                siblingSpouses.forEach(spouse => {
                    if (!visited.has(spouse.Id)) {
                        visited.add(spouse.Id);
                        personGeneration.set(spouse.Id, focusGeneration);
                        this.generations.get(focusGeneration).push(spouse);
                    }
                });
            }
        });

        // Traverse ancestors from focus person (parents, grandparents, etc.)
        // Both parents are ancestors, so no need to add "spouses" separately
        this.traverseAncestors(focusPerson, focusGeneration - 1, visited, personGeneration);

        // Traverse descendants ONLY from focus person (not from siblings)
        // This excludes nephews/nieces (siblings' children)
        this.traverseDescendants(focusPerson, focusGeneration + 1, visited, personGeneration);
    }

    /**
     * Traverse ancestors (parents, grandparents, etc.)
     * Both parents are ancestors of the child, so we just add all parents
     * No need to separately handle "spouses" - they're already both parents
     */
    traverseAncestors(person, generation, visited, personGeneration) {
        const parents = this.getParents(person);

        parents.forEach(parent => {
            if (!visited.has(parent.Id)) {
                visited.add(parent.Id);
                personGeneration.set(parent.Id, generation);

                if (!this.generations.has(generation)) {
                    this.generations.set(generation, []);
                }
                this.generations.get(generation).push(parent);

                // Recursively traverse grandparents
                this.traverseAncestors(parent, generation - 1, visited, personGeneration);
            }
        });
    }

    /**
     * Traverse descendants (children, grandchildren, etc.)
     * Add children and their spouses
     */
    traverseDescendants(person, generation, visited, personGeneration) {
        const children = this.getChildren(person);

        children.forEach(child => {
            if (!visited.has(child.Id)) {
                visited.add(child.Id);
                personGeneration.set(child.Id, generation);

                if (!this.generations.has(generation)) {
                    this.generations.set(generation, []);
                }
                this.generations.get(generation).push(child);

                // Add child's spouse(s) to same generation
                const childSpouses = this.getSpouses(child);
                childSpouses.forEach(spouse => {
                    if (!visited.has(spouse.Id)) {
                        visited.add(spouse.Id);
                        personGeneration.set(spouse.Id, generation);
                        this.generations.get(generation).push(spouse);
                    }
                });

                // Recursively traverse grandchildren
                this.traverseDescendants(child, generation + 1, visited, personGeneration);
            }
        });
    }

    /**
     * Position all generations as aligned rows
     */
    positionAllGenerations(focusPerson, nodes, edges) {
        // Sort generation keys (negative to positive)
        const genKeys = Array.from(this.generations.keys()).sort((a, b) => a - b);

        genKeys.forEach(generation => {
            const people = this.generations.get(generation);
            const y = generation * this.rowHeight;

            if (generation === 0) {
                // Focus person's generation - special positioning
                this.positionFocusGeneration(focusPerson, people, y, nodes, edges);
            } else if (generation < 0) {
                // Ancestor generations
                this.positionAncestorGeneration(people, y, nodes, edges);
            } else {
                // Descendant generations
                this.positionDescendantGeneration(people, y, nodes, edges);
            }
        });

        // Add all edges after positioning
        this.addAllEdges(nodes, edges);
    }


    /**
     * Position the focus person's generation (generation 0)
     * Includes focus person, their spouse(s), and their siblings (but NOT ancestors' siblings)
     */
    positionFocusGeneration(focusPerson, people, y, nodes, edges) {
        // Focus person at center
        let currentX = 0;
        this.addNode(focusPerson, currentX, y, 'primary', nodes);
        currentX += this.nodeSpacing.x;

        // Spouses to the right, slightly above
        const spouses = this.getSpouses(focusPerson);
        spouses.forEach(spouse => {
            this.addNode(spouse, currentX, y + this.spouseYOffset, 'spouse', nodes);
            currentX += this.nodeSpacing.x;
        });

        // Siblings to the right of spouses (ONLY for the focus person)
        const siblings = this.getSiblings(focusPerson);
        siblings.forEach(sibling => {
            if (!this.nodePositions.has(sibling.Id)) {
                this.addNode(sibling, currentX, y, 'sibling', nodes);

                // Add sibling's spouse(s) slightly above
                const sibSpouses = this.getSpouses(sibling);
                sibSpouses.forEach(sibSpouse => {
                    if (!this.nodePositions.has(sibSpouse.Id)) {
                        currentX += this.nodeSpacing.x;
                        this.addNode(sibSpouse, currentX, y + this.spouseYOffset, 'spouse', nodes);
                    }
                });

                currentX += this.nodeSpacing.x;
            }
        });
    }

    /**
     * Position ancestor generation (parents, grandparents, etc.)
     */
    positionAncestorGeneration(people, y, nodes, edges) {
        // Sort people by their children's positions to maintain family grouping
        const sortedPeople = this.sortByChildrenPositions(people);

        let currentX = -(sortedPeople.length * this.nodeSpacing.x) / 2;

        sortedPeople.forEach(person => {
            if (!this.nodePositions.has(person.Id)) {
                this.addNode(person, currentX, y, 'ancestor', nodes);

                // Add spouse(s) slightly above
                const spouses = this.getSpouses(person);
                spouses.forEach(spouse => {
                    if (!this.nodePositions.has(spouse.Id)) {
                        currentX += this.nodeSpacing.x;
                        this.addNode(spouse, currentX, y + this.spouseYOffset, 'ancestor', nodes);
                    }
                });

                currentX += this.nodeSpacing.x;
            }
        });
    }

    /**
     * Position descendant generation (children, grandchildren, etc.)
     */
    positionDescendantGeneration(people, y, nodes, edges) {
        // Sort children by birth date within each family group
        const sortedPeople = this.sortByParentPositionsAndAge(people);

        let currentX = -(sortedPeople.length * this.nodeSpacing.x) / 2;

        sortedPeople.forEach(person => {
            if (!this.nodePositions.has(person.Id)) {
                this.addNode(person, currentX, y, 'descendant', nodes);

                // Add spouse(s) slightly above
                const spouses = this.getSpouses(person);
                spouses.forEach(spouse => {
                    if (!this.nodePositions.has(spouse.Id)) {
                        currentX += this.nodeSpacing.x;
                        this.addNode(spouse, currentX, y + this.spouseYOffset, 'spouse', nodes);
                    }
                });

                currentX += this.nodeSpacing.x;
            }
        });
    }

    /**
     * Sort people by their children's average position (for ancestors)
     */
    sortByChildrenPositions(people) {
        return people.sort((a, b) => {
            const aChildren = this.getChildren(a);
            const bChildren = this.getChildren(b);

            const aAvgX = this.getAverageXPosition(aChildren);
            const bAvgX = this.getAverageXPosition(bChildren);

            return aAvgX - bAvgX;
        });
    }

    /**
     * Sort people by their parents' position and then by birth date (for descendants)
     */
    sortByParentPositionsAndAge(people) {
        return people.sort((a, b) => {
            const aParents = this.getParents(a);
            const bParents = this.getParents(b);

            // If they share the same parent, sort by age
            const sharedParent = aParents.find(ap => bParents.find(bp => bp.Id === ap.Id));
            if (sharedParent) {
                return this.compareByAge(a, b);
            }

            // Otherwise sort by parent position
            const aAvgX = this.getAverageXPosition(aParents);
            const bAvgX = this.getAverageXPosition(bParents);

            return aAvgX - bAvgX;
        });
    }

    /**
     * Get average X position of a list of people
     */
    getAverageXPosition(people) {
        if (people.length === 0) return 0;

        const positions = people
            .filter(p => this.nodePositions.has(p.Id))
            .map(p => this.nodePositions.get(p.Id).x);

        if (positions.length === 0) return 0;

        const sum = positions.reduce((acc, x) => acc + x, 0);
        return sum / positions.length;
    }

    /**
     * Add all edges between positioned nodes
     */
    addAllEdges(nodes, edges) {
        nodes.forEach(node => {
            const person = this.findPerson(node.data.id);
            if (!person) return;

            // Parent-child edges
            const children = this.getChildren(person);
            children.forEach(child => {
                if (this.nodePositions.has(child.Id)) {
                    this.addParentChildEdge(person.Id, child.Id, edges);
                }
            });

            // Spouse edges
            const spouses = this.getSpouses(person);
            spouses.forEach(spouse => {
                if (this.nodePositions.has(spouse.Id)) {
                    this.addSpouseEdge(person.Id, spouse.Id, edges);
                }
            });
        });
    }

    /**
     * Add a node to the layout
     */
    addNode(person, x, y, type, nodes) {
        if (this.nodePositions.has(person.Id)) {
            return; // Already positioned
        }

        this.nodePositions.set(person.Id, { x, y });

        nodes.push({
            data: {
                id: person.Id,
                label: person.FullName || `${person.FirstName} ${person.LastName}`,
                person: person,
                type: type
            },
            position: { x, y },
            classes: type
        });
    }

    /**
     * Add parent-child edge
     */
    addParentChildEdge(parentId, childId, edges) {
        edges.push({
            data: {
                id: `${parentId}_to_${childId}`,
                source: parentId,
                target: childId,
                type: 'parent-child'
            }
        });
    }

    /**
     * Add spouse edge (arc/curved line)
     */
    addSpouseEdge(person1Id, person2Id, edges) {
        // Use alphabetical order to prevent duplicates
        const [source, target] = person1Id < person2Id ? [person1Id, person2Id] : [person2Id, person1Id];
        
        edges.push({
            data: {
                id: `${source}_spouse_${target}`,
                source: source,
                target: target,
                type: 'spouse'
            }
        });
    }

    /**
     * Sort children by age (oldest first)
     */
    sortChildrenByAge(children) {
        return [...children].sort((a, b) => this.compareByAge(a, b));
    }

    /**
     * Compare two people by age (oldest first)
     */
    compareByAge(personA, personB) {
        const aDate = personA.BirthDate ? new Date(personA.BirthDate).getTime() : 0;
        const bDate = personB.BirthDate ? new Date(personB.BirthDate).getTime() : 0;
        return aDate - bDate; // Oldest first (earliest date)
    }

    /**
     * Helper: Find person by ID
     */
    findPerson(personId) {
        return this.familyData.PeopleCollection.find(p => p.Id === personId);
    }

    /**
     * Helper: Get spouses
     */
    getSpouses(person) {
        if (!person || !person.Relatives) return [];
        return person.Relatives
            .filter(rel => rel.RelationType === 'Spouse')
            .map(rel => this.findPerson(rel.PersonId))
            .filter(p => p != null);
    }

    /**
     * Helper: Get parents
     */
    getParents(person) {
        if (!person || !person.Relatives) return [];
        return person.Relatives
            .filter(rel => rel.RelationType === 'Parent')
            .map(rel => this.findPerson(rel.PersonId))
            .filter(p => p != null);
    }

    /**
     * Helper: Get children
     */
    getChildren(person) {
        if (!person || !person.Relatives) return [];
        return person.Relatives
            .filter(rel => rel.RelationType === 'Child')
            .map(rel => this.findPerson(rel.PersonId))
            .filter(p => p != null);
    }

    /**
     * Helper: Get siblings
     */
    getSiblings(person) {
        if (!person || !person.Relatives) return [];
        return person.Relatives
            .filter(rel => rel.RelationType === 'Sibling')
            .map(rel => this.findPerson(rel.PersonId))
            .filter(p => p != null);
    }
}

// Export for use in Tree.cshtml
window.FamilyTreeLayout = FamilyTreeLayout;
