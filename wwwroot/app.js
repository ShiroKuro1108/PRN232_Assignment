// Global variables
let products = [];
let currentEditingId = null;

// API base URL - adjust this based on your deployment
const API_BASE_URL = '/api/products';

// Initialize the app
document.addEventListener('DOMContentLoaded', function() {
    loadProducts();
    setupEventListeners();
});

// Setup event listeners
function setupEventListeners() {
    document.getElementById('productForm').addEventListener('submit', handleFormSubmit);
}

// Show/Hide pages
function showHome() {
    document.getElementById('homePage').style.display = 'block';
    document.getElementById('productDetailPage').style.display = 'none';
    document.getElementById('addProductPage').style.display = 'none';
    loadProducts();
}

function showProductDetail(id) {
    document.getElementById('homePage').style.display = 'none';
    document.getElementById('productDetailPage').style.display = 'block';
    document.getElementById('addProductPage').style.display = 'none';
    loadProductDetail(id);
}

function showAddProduct() {
    document.getElementById('homePage').style.display = 'none';
    document.getElementById('productDetailPage').style.display = 'none';
    document.getElementById('addProductPage').style.display = 'block';
    
    // Reset form
    document.getElementById('productForm').reset();
    document.getElementById('productId').value = '';
    document.getElementById('formTitle').textContent = 'Add New Product';
    document.getElementById('submitBtn').textContent = 'Add Product';
    currentEditingId = null;
}

function showEditProduct(id) {
    const product = products.find(p => p.id === id);
    if (!product) return;
    
    document.getElementById('homePage').style.display = 'none';
    document.getElementById('productDetailPage').style.display = 'none';
    document.getElementById('addProductPage').style.display = 'block';
    
    // Fill form with product data
    document.getElementById('productId').value = product.id;
    document.getElementById('productName').value = product.name;
    document.getElementById('productDescription').value = product.description;
    document.getElementById('productPrice').value = product.price;
    document.getElementById('productImage').value = product.image || '';
    
    document.getElementById('formTitle').textContent = 'Edit Product';
    document.getElementById('submitBtn').textContent = 'Update Product';
    currentEditingId = id;
}

// API calls
async function loadProducts() {
    try {
        showLoading(true);
        const response = await fetch(API_BASE_URL);
        if (!response.ok) throw new Error('Failed to load products');
        
        products = await response.json();
        displayProducts(products);
    } catch (error) {
        console.error('Error loading products:', error);
        showAlert('Error loading products. Please try again.', 'danger');
    } finally {
        showLoading(false);
    }
}

async function loadProductDetail(id) {
    try {
        showLoading(true);
        const response = await fetch(`${API_BASE_URL}/${id}`);
        if (!response.ok) throw new Error('Failed to load product');
        
        const product = await response.json();
        displayProductDetail(product);
    } catch (error) {
        console.error('Error loading product detail:', error);
        showAlert('Error loading product details. Please try again.', 'danger');
    } finally {
        showLoading(false);
    }
}

async function handleFormSubmit(event) {
    event.preventDefault();
    
    const formData = {
        name: document.getElementById('productName').value,
        description: document.getElementById('productDescription').value,
        price: parseFloat(document.getElementById('productPrice').value),
        image: document.getElementById('productImage').value || null
    };
    
    try {
        showLoading(true);
        let response;
        
        if (currentEditingId) {
            // Update existing product
            formData.id = currentEditingId;
            response = await fetch(`${API_BASE_URL}/${currentEditingId}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData)
            });
        } else {
            // Create new product
            response = await fetch(API_BASE_URL, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(formData)
            });
        }
        
        if (!response.ok) throw new Error('Failed to save product');
        
        showAlert(currentEditingId ? 'Product updated successfully!' : 'Product created successfully!', 'success');
        showHome();
    } catch (error) {
        console.error('Error saving product:', error);
        showAlert('Error saving product. Please try again.', 'danger');
    } finally {
        showLoading(false);
    }
}

async function deleteProduct(id) {
    if (!confirm('Are you sure you want to delete this product?')) return;

    try {
        showLoading(true);
        const response = await fetch(`${API_BASE_URL}/${id}`, {
            method: 'DELETE'
        });

        if (!response.ok) throw new Error('Failed to delete product');

        showAlert('Product deleted successfully!', 'success');
        loadProducts();
    } catch (error) {
        console.error('Error deleting product:', error);
        showAlert('Error deleting product. Please try again.', 'danger');
    } finally {
        showLoading(false);
    }
}

// Display functions
function displayProducts(productsToShow) {
    const container = document.getElementById('productsContainer');

    if (productsToShow.length === 0) {
        container.innerHTML = `
            <div class="col-12">
                <div class="no-products">
                    <div>📦</div>
                    <h4>No products found</h4>
                    <p>Try adjusting your search or add some products to get started.</p>
                </div>
            </div>
        `;
        return;
    }

    container.innerHTML = productsToShow.map(product => `
        <div class="col-md-4 col-sm-6 mb-4">
            <div class="card product-card h-100" onclick="showProductDetail(${product.id})">
                <div class="card-img-top">
                    ${product.image ?
                        `<img src="${product.image}" class="product-image" alt="${product.name}" onerror="this.parentElement.innerHTML='<div class=\\'product-image-placeholder\\'>No Image Available</div>'">` :
                        `<div class="product-image-placeholder">No Image Available</div>`
                    }
                </div>
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title">${escapeHtml(product.name)}</h5>
                    <p class="card-text flex-grow-1">${escapeHtml(product.description.substring(0, 100))}${product.description.length > 100 ? '...' : ''}</p>
                    <div class="mt-auto">
                        <div class="price-tag mb-2">$${product.price.toFixed(2)}</div>
                        <div class="product-actions" onclick="event.stopPropagation()">
                            <button class="btn btn-primary btn-sm" onclick="showProductDetail(${product.id})">View Details</button>
                            <button class="btn btn-warning btn-sm" onclick="showEditProduct(${product.id})">Edit</button>
                            <button class="btn btn-danger btn-sm" onclick="deleteProduct(${product.id})">Delete</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    `).join('');
}

function displayProductDetail(product) {
    const container = document.getElementById('productDetailContent');
    container.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                ${product.image ?
                    `<img src="${product.image}" class="product-detail-image rounded" alt="${product.name}" onerror="this.parentElement.innerHTML='<div class=\\'product-detail-image-placeholder rounded\\'>No Image Available</div>'">` :
                    `<div class="product-detail-image-placeholder rounded">No Image Available</div>`
                }
            </div>
            <div class="col-md-6">
                <h1>${escapeHtml(product.name)}</h1>
                <p class="lead">${escapeHtml(product.description)}</p>
                <div class="price-tag mb-4">$${product.price.toFixed(2)}</div>
                <div class="mb-3">
                    <small class="text-muted">
                        Created: ${new Date(product.createdAt).toLocaleDateString()}<br>
                        Updated: ${new Date(product.updatedAt).toLocaleDateString()}
                    </small>
                </div>
                <div class="d-grid gap-2 d-md-flex">
                    <button class="btn btn-warning" onclick="showEditProduct(${product.id})">Edit Product</button>
                    <button class="btn btn-danger" onclick="deleteProduct(${product.id})">Delete Product</button>
                </div>
            </div>
        </div>
    `;
}

// Search and filter functions
function searchProducts() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const filteredProducts = products.filter(product =>
        product.name.toLowerCase().includes(searchTerm) ||
        product.description.toLowerCase().includes(searchTerm)
    );
    displayProducts(filteredProducts);
}

function sortProducts() {
    const sortBy = document.getElementById('sortSelect').value;
    let sortedProducts = [...products];

    switch (sortBy) {
        case 'name':
            sortedProducts.sort((a, b) => a.name.localeCompare(b.name));
            break;
        case 'price-low':
            sortedProducts.sort((a, b) => a.price - b.price);
            break;
        case 'price-high':
            sortedProducts.sort((a, b) => b.price - a.price);
            break;
        default:
            sortedProducts = products;
    }

    displayProducts(sortedProducts);
}

// Utility functions
function showLoading(show) {
    document.getElementById('loadingSpinner').style.display = show ? 'block' : 'none';
}

function showAlert(message, type = 'info') {
    // Remove existing alerts
    const existingAlerts = document.querySelectorAll('.alert');
    existingAlerts.forEach(alert => alert.remove());

    // Create new alert
    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    // Insert at the top of the container
    const container = document.querySelector('.container');
    container.insertBefore(alertDiv, container.firstChild);

    // Auto-dismiss after 5 seconds
    setTimeout(() => {
        if (alertDiv.parentNode) {
            alertDiv.remove();
        }
    }, 5000);
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
