import { useNavigate } from 'react-router-dom';
import { useCart } from '../context/CartContext';

export default function CartPage() {
  const { items, updateQuantity, updateNotes, removeItem, total, restaurantSlug } = useCart();
  const navigate = useNavigate();

  if (items.length === 0) {
    return (
      <div className="page-center">
        <h2>Carrinho vazio</h2>
        <button className="btn-primary" onClick={() => navigate(`/menu/${restaurantSlug}`)}>
          Voltar ao cardapio
        </button>
      </div>
    );
  }

  return (
    <div className="cart-page">
      <h1>Seu Pedido</h1>

      <div className="cart-items">
        {items.map(item => (
          <div key={item.menuItemId} className="cart-item">
            <div className="cart-item-header">
              <h3>{item.name}</h3>
              <button className="btn-remove" onClick={() => removeItem(item.menuItemId)}>X</button>
            </div>
            <div className="cart-item-controls">
              <div className="qty-controls">
                <button onClick={() => updateQuantity(item.menuItemId, item.quantity - 1)}>-</button>
                <span>{item.quantity}</span>
                <button onClick={() => updateQuantity(item.menuItemId, item.quantity + 1)}>+</button>
              </div>
              <span className="cart-item-price">R$ {(item.price * item.quantity).toFixed(2)}</span>
            </div>
            <input
              type="text"
              placeholder="Observacoes..."
              value={item.notes}
              onChange={e => updateNotes(item.menuItemId, e.target.value)}
              className="cart-notes"
            />
          </div>
        ))}
      </div>

      <div className="cart-total">
        <span>Total</span>
        <span>R$ {total.toFixed(2)}</span>
      </div>

      <button className="btn-primary" onClick={() => navigate('/checkout')}>
        Finalizar Pedido
      </button>
    </div>
  );
}
