import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'https://localhost:7207';

const ProductList = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchTerm, setSearchTerm] = useState('');
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [productToDelete, setProductToDelete] = useState(null);
  const [deleting, setDeleting] = useState(false);

  useEffect(() => {
    fetchProducts();
  }, []);

  const fetchProducts = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/api/products`);
      setProducts(response.data);
      setLoading(false);
    } catch (err) {
      setError('Failed to fetch products');
      setLoading(false);
    }
  };

  const handleDeleteClick = (product) => {
    setProductToDelete(product);
    setShowDeleteModal(true);
  };

  const handleDeleteConfirm = async () => {
    if (!productToDelete) return;
    
    setDeleting(true);
    try {
      await axios.delete(`${API_BASE_URL}/api/products/${productToDelete.id}`);
      setProducts(products.filter(product => product.id !== productToDelete.id));
      setShowDeleteModal(false);
      setProductToDelete(null);
    } catch (err) {
      setError('Failed to delete product');
    } finally {
      setDeleting(false);
    }
  };

  const handleDeleteCancel = () => {
    setShowDeleteModal(false);
    setProductToDelete(null);
  };

  const filteredProducts = products.filter(product =>
    product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    product.description.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (loading) return <div className="loading">Loading products...</div>;
  if (error) return <div className="error">{error}</div>;

  return (
    <div className="product-list-container">
      {/* Delete Confirmation Modal */}
      {showDeleteModal && productToDelete && (
        <div className="modal-overlay" onClick={handleDeleteCancel}>
          <div className="delete-modal" onClick={(e) => e.stopPropagation()}>
            <div className="modal-header">
              <h3>Delete Confirmation</h3>
              <button className="close-button" onClick={handleDeleteCancel}>×</button>
            </div>
            <div className="modal-body">
              <p className="modal-message">
                Are you sure you want to delete the product <span className="product-name">"{productToDelete.name}"</span>?
              </p>
            </div>
            <div className="modal-actions">
              <button 
                onClick={handleDeleteCancel} 
                className="btn btn-secondary modal-btn"
                disabled={deleting}
              >
                Cancel
              </button>
              <button 
                onClick={handleDeleteConfirm} 
                className="btn btn-danger modal-btn"
                disabled={deleting}
              >
                {deleting ? 'Deleting...' : 'Delete'}
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="header">
        <h2>⚡ CYBER PRODUCTS ⚡</h2>
        <div className="search-container">
          <input
            type="text"
            placeholder="🔍 Search the matrix..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="search-input"
          />
        </div>
      </div>

      {filteredProducts.length === 0 ? (
        <div className="no-products">
          <p>🤖 No products found in the database...</p>
          <Link to="/products/new" className="btn btn-primary">⚡ Create First Product</Link>
        </div>
      ) : (
        <div className="products-grid">
          {filteredProducts.map((product) => (
            <div key={product.id} className="product-card">
              <div className="product-image">
                {product.imageUrl ? (
                  <img src={product.imageUrl} alt={product.name} />
                ) : (
                  <div className="no-image">📷 NO DATA</div>
                )}
              </div>
              <div className="product-info">
                <h3>{product.name}</h3>
                <p className="price">💰 ${product.price}</p>
                <p className="description">
                  {product.description.length > 100
                    ? product.description.substring(0, 100) + '...'
                    : product.description}
                </p>
                <div className="product-actions">
                  <Link to={`/products/${product.id}`} className="btn btn-primary">
                    🔍 View Details
                  </Link>
                  <Link to={`/products/edit/${product.id}`} className="btn btn-secondary">
                    ✏️ Edit
                  </Link>
                  <button
                    onClick={() => handleDeleteClick(product)}
                    className="btn btn-danger"
                    disabled={deleting && productToDelete?.id === product.id}
                  >
                    {deleting && productToDelete?.id === product.id ? '⏳ Deleting...' : '🗑️ Delete'}
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default ProductList;