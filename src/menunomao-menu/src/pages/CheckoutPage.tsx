import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../api/client';
import { useCart } from '../context/CartContext';
import { useOrder } from '../context/OrderContext';

export default function CheckoutPage() {
  const { items, tableToken, clearCart } = useCart();
  const { setOrderId } = useOrder();
  const navigate = useNavigate();
  const [name, setName] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleSubmit = async () => {
    if (items.length === 0) return;
    setLoading(true);
    setError('');

    try {
      // First resolve the table
      const tableRes = await api.get(`/tables/resolve?token=${tableToken}`);
      const tableId = tableRes.data.id;

      const res = await api.post('/orders', {
        tableId,
        customerName: name || null,
        items: items.map(i => ({
          menuItemId: i.menuItemId,
          quantity: i.quantity,
          notes: i.notes || null,
        })),
      });

      setOrderId(res.data.id);
      clearCart();
      navigate(`/order/${res.data.id}`);
    } catch {
      setError('Erro ao enviar pedido. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="checkout-page">
      <h1>Confirmar Pedido</h1>

      <div className="form-field">
        <label>Seu nome (opcional)</label>
        <input type="text" value={name} onChange={e => setName(e.target.value)} placeholder="Nome" />
      </div>

      <div className="checkout-summary">
        <h3>{items.length} {items.length === 1 ? 'item' : 'itens'}</h3>
        <ul>
          {items.map(i => (
            <li key={i.menuItemId}>{i.quantity}x {i.name} — R$ {(i.price * i.quantity).toFixed(2)}</li>
          ))}
        </ul>
      </div>

      {error && <p className="error-msg">{error}</p>}

      <button className="btn-primary" onClick={handleSubmit} disabled={loading}>
        {loading ? 'Enviando...' : 'Enviar Pedido'}
      </button>
    </div>
  );
}
