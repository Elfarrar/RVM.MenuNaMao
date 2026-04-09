import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import api from '../api/client';

interface OrderData {
  id: string;
  status: string;
  customerName: string | null;
  tableNumber: number;
  totalAmount: number;
  items: { menuItemName: string; quantity: number; unitPrice: number; status: string }[];
}

export default function OrderStatusPage() {
  const { orderId } = useParams<{ orderId: string }>();
  const [order, setOrder] = useState<OrderData | null>(null);

  useEffect(() => {
    const fetchOrder = () => {
      api.get(`/orders/${orderId}`).then(res => setOrder(res.data));
    };

    fetchOrder();
    const interval = setInterval(fetchOrder, 10000);
    return () => clearInterval(interval);
  }, [orderId]);

  if (!order) return <div className="loading">Carregando...</div>;

  const statusLabels: Record<string, string> = {
    Pending: 'Pendente',
    Preparing: 'Preparando',
    Ready: 'Pronto',
    Delivered: 'Entregue',
    Cancelled: 'Cancelado',
  };

  return (
    <div className="order-status-page">
      <h1>Pedido #{order.id.slice(0, 8)}</h1>

      <div className="status-badge-lg">{statusLabels[order.status] || order.status}</div>

      <div className="order-details">
        <p>Mesa: {order.tableNumber}</p>
        {order.customerName && <p>Nome: {order.customerName}</p>}
      </div>

      <div className="order-items-list">
        {order.items.map((item, idx) => (
          <div key={idx} className="order-item-row">
            <span>{item.quantity}x {item.menuItemName}</span>
            <span>R$ {(item.unitPrice * item.quantity).toFixed(2)}</span>
          </div>
        ))}
      </div>

      <div className="order-total">
        <span>Total</span>
        <span>R$ {order.totalAmount.toFixed(2)}</span>
      </div>

      <p className="refresh-note">Atualizando automaticamente a cada 10s</p>
    </div>
  );
}
