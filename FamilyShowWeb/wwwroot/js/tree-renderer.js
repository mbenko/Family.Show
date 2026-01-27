// FamilyShow Tree Renderer - SVG Icons and Visual Enhancements
// Converts WPF person icons to SVG for cytoscape rendering

// Male person icon (from WPF PersonFigureFill geometry)
const malePersonSVG = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22.5 43.5" width="30" height="58">
  <!-- Head -->
  <ellipse cx="11" cy="4.5" rx="4.5" ry="4.5" fill="COLOR_FILL"/>
  <!-- Body and legs -->
  <path d="M 12.05,25.94 L 12.05,41.49 C 12.05,42.82 13.99,43.53 14.81,43.53 C 15.62,43.53 17.05,43.43 17.05,41.70 L 17.05,15.52 L 18.59,15.52 L 18.59,24.92 C 18.59,26.14 19.81,26.45 20.53,26.45 C 21.24,26.45 22.47,26.35 22.47,25.12 C 22.47,23.90 22.16,16.24 22.16,14.20 C 22.16,12.15 20.73,9.50 17.26,9.50 L 5.21,9.50 C 1.74,9.50 0.31,12.15 0.31,14.20 C 0.31,16.24 0,23.90 0,25.12 C 0,26.35 1.22,26.45 1.94,26.45 C 2.65,26.45 3.88,26.14 3.88,24.92 L 3.88,15.52 L 5.41,15.52 L 5.41,41.70 C 5.41,43.43 6.84,43.53 7.66,43.53 C 8.48,43.53 10.42,42.82 10.42,41.49 L 10.42,25.94 L 12.05,25.94 Z" fill="COLOR_FILL"/>
</svg>`;

// Female person icon (skirt variant - simplified from male)
const femalePersonSVG = `
<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 22.5 43.5" width="30" height="58">
  <!-- Head -->
  <ellipse cx="11" cy="4.5" rx="4.5" ry="4.5" fill="COLOR_FILL"/>
  <!-- Body with dress/skirt -->
  <path d="M 11.2,9.5 L 11.2,25 L 8,25 L 8,41.5 C 8,42.5 9,43 9.5,43 C 10,43 10.5,42.8 10.5,41.5 L 10.5,26 L 11.9,26 L 11.9,41.5 C 11.9,42.8 12.4,43 12.9,43 C 13.4,43 14.4,42.5 14.4,41.5 L 14.4,25 L 11.2,25 Z M 5.5,9.5 C 2.5,9.5 1.5,11.5 1.5,13 L 1.5,15 L 1,24.5 C 1,24.5 4,25 11.2,25 C 18.4,25 21.5,24.5 21.5,24.5 L 21,15 L 21,13 C 21,11.5 20,9.5 17,9.5 Z" fill="COLOR_FILL"/>
</svg>`;

// Color scheme matching WPF diagram
const nodeColors = {
    // Living persons (brighter colors)
    living: {
        male: '#FF6B35',      // Orange for living males
        female: '#FF1493',    // Deep pink for living females  
        primary: '#FFD700'    // Gold for primary focus
    },
    // Deceased persons (blue/gray tones)
    deceased: {
        male: '#4682B4',      // Steel blue for deceased males
        female: '#BA55D3',    // Medium orchid for deceased females
    },
    // Relationship-based colors
    related: '#007bff',       // Blue for related
    spouse: '#fd7e14',        // Orange for spouse
    sibling: '#20c997',       // Teal for sibling
    ancestor: '#28a745',      // Green for ancestor
    descendant: '#6f42c1'     // Purple for descendant
};

/**
 * Generate node color based on person data and relationship
 */
function getNodeColor(person, nodeClass) {
    const isDeceased = person.DeathDate != null;
    const isMale = !person.Gender || person.Gender === 'Male' || person.Gender === 0; // Handle enum as number or string

    // Primary focused node
    if (nodeClass && nodeClass.includes('focused')) {
        return nodeColors.living.primary;
    }

    // Relationship-based colors override gender/alive status
    if (nodeClass) {
        if (nodeClass.includes('ancestor')) return nodeColors.ancestor;
        if (nodeClass.includes('descendant')) return nodeColors.descendant;
        if (nodeClass.includes('spouse')) return nodeColors.spouse;
        if (nodeClass.includes('sibling')) return nodeColors.sibling;
    }

    // Gender and living status
    if (isDeceased) {
        return isMale ? nodeColors.deceased.male : nodeColors.deceased.female;
    } else {
        return isMale ? nodeColors.living.male : nodeColors.living.female;
    }
}

/**
 * Generate SVG data URL for a person node
 */
function generatePersonSVG(person, nodeClass) {
    const isMale = !person.Gender || person.Gender === 'Male' || person.Gender === 0;
    const template = isMale ? malePersonSVG : femalePersonSVG;
    const color = getNodeColor(person, nodeClass);

    const svg = template.replace(/COLOR_FILL/g, color);
    return 'data:image/svg+xml;charset=utf-8,' + encodeURIComponent(svg);
}

/**
 * Update cytoscape node with SVG background image
 */
function updateNodeWithSVG(cy, nodeId, person, nodeClass) {
    const node = cy.getElementById(nodeId);
    if (node && node.length > 0) {
        const svgUrl = generatePersonSVG(person, nodeClass);
        node.style({
            'background-image': svgUrl,
            'background-fit': 'contain',
            'background-clip': 'none',
            'border-width': '2px',
            'border-color': getNodeColor(person, nodeClass)
        });
    }
}

/**
 * Apply SVG rendering to all nodes in the graph
 */
function applySVGToAllNodes(cy, familyData) {
    cy.nodes().forEach(node => {
        const personId = node.id();
        const person = familyData.PeopleCollection.find(p => p.Id === personId);
        
        if (person) {
            // Determine node class from cytoscape classes
            let nodeClass = '';
            if (node.hasClass('focused')) nodeClass += ' focused';
            if (node.hasClass('ancestor')) nodeClass += ' ancestor';
            if (node.hasClass('descendant')) nodeClass += ' descendant';
            if (node.hasClass('spouse')) nodeClass += ' spouse';
            if (node.hasClass('sibling')) nodeClass += ' sibling';
            
            updateNodeWithSVG(cy, personId, person, nodeClass);
        }
    });
}

/**
 * Enhanced cytoscape stylesheet with SVG support
 */
function getEnhancedCytoscapeStyle() {
    return [
        { 
            selector: 'node', 
            style: { 
                'label': 'data(label)', 
                'color': '#fff', 
                'text-valign': 'bottom', 
                'text-halign': 'center',
                'text-margin-y': 5,
                'width': '50px',
                'height': '70px',
                'shape': 'rectangle',
                'font-size': '11px',
                'font-weight': '600',
                'text-wrap': 'wrap',
                'text-max-width': '80px',
                'background-color': '#333',
                'border-width': '2px',
                'border-color': '#555',
                'text-background-color': 'rgba(0,0,0,0.7)',
                'text-background-opacity': 1,
                'text-background-padding': '3px',
                'text-background-shape': 'roundrectangle',
                'transition-property': 'border-color, border-width',
                'transition-duration': '0.3s'
            } 
        },
        {
            selector: 'node.focused',
            style: {
                'border-width': '4px',
                'border-color': '#FFD700',
                'width': '60px',
                'height': '85px',
                'font-size': '13px',
                'font-weight': 'bold',
                'z-index': 100
            }
        },
        {
            selector: 'node:selected',
            style: {
                'border-width': '3px',
                'overlay-color': '#FFD700',
                'overlay-opacity': 0.3,
                'overlay-padding': '5px'
            }
        },
        { 
            selector: 'edge', 
            style: { 
                'width': 2, 
                'line-color': '#666', 
                'target-arrow-color': '#666', 
                'target-arrow-shape': 'triangle',
                'curve-style': 'bezier',
                'arrow-scale': 1.2,
                'transition-property': 'line-color, target-arrow-color, width',
                'transition-duration': '0.3s'
            } 
        },
        {
            selector: 'edge[type="parent-child"]',
            style: {
                'line-color': '#888',
                'target-arrow-color': '#888',
                'width': 3,
                'line-style': 'solid'
            }
        },
        {
            selector: 'edge[type="spouse"]',
            style: {
                'line-color': '#fd7e14',
                'target-arrow-shape': 'none',
                'line-style': 'dashed',
                'line-dash-pattern': [6, 3],
                'width': 2
            }
        },
        {
            selector: 'edge.hidden',
            style: {
                'opacity': 0.1
            }
        },
        {
            selector: 'node.hidden',
            style: {
                'opacity': 0.2
            }
        }
    ];
}

// Export functions for use in Tree.cshtml
if (typeof window !== 'undefined') {
    window.TreeRenderer = {
        generatePersonSVG,
        updateNodeWithSVG,
        applySVGToAllNodes,
        getEnhancedCytoscapeStyle,
        getNodeColor,
        nodeColors
    };
}
