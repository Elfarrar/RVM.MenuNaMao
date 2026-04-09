import { useEffect, useState } from 'react';
import { useParams, useSearchParams, useNavigate } from 'react-router-dom';
import api from '../api/client';
import { useCart } from '../context/CartContext';

interface MenuItemData {
  id: string;
  name: string;
  description: string | null;
  price: number;
  imageUrl: string | null;
  preparationTimeMinutes: number;
}

interface CategoryData {
  id: string;
  name: string;
  items: MenuItemData[];
}

interface MenuData {
  restaurant: { name: string; slug: string };
  categories: CategoryData[];
}

export default function MenuPage() {
  const { slug } = useParams<{ slug: string }>();
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const { addItem, items, setTableToken, setRestaurantSlug } = useCart();
  const [menu, setMenu] = useState<MenuData | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const token = searchParams.get('table') || '';
    if (token) setTableToken(token);
    if (slug) setRestaurantSlug(slug);

    api.get(`/menu/${slug}`).then(res => {
      setMenu(res.data);
      setLoading(false);
    });
  }, [slug, searchParams, setTableToken, setRestaurantSlug]);

  if (loading) return <div className="loading">Carregando cardapio...</div>;
  if (!menu) return <div className="loading">Restaurante nao encontrado.</div>;

  const cartCount = items.reduce((s, i) => s + i.quantity, 0);

  return (
    <div className="menu-page">
      <header className="menu-header">
        <h1>{menu.restaurant.name}</h1>
      </header>

      {menu.categories.map(cat => (
        <section key={cat.id} className="menu-category">
          <h2>{cat.name}</h2>
          <div className="menu-items">
            {cat.items.map(item => (
              <div key={item.id} className="menu-item-card">
                <div className="item-info">
                  <h3>{item.name}</h3>
                  {item.description && <p className="item-desc">{item.description}</p>}
                  <div className="item-meta">
                    <span className="item-price">R$ {item.price.toFixed(2)}</span>
                    <span className="item-time">{item.preparationTimeMinutes} min</span>
                  </div>
                </div>
                <button className="btn-add" onClick={() => addItem({ menuItemId: item.id, name: item.name, price: item.price })}>
                  +
                </button>
              </div>
            ))}
          </div>
        </section>
      ))}

      {cartCount > 0 && (
        <button className="cart-fab" onClick={() => navigate('/cart')}>
          Carrinho ({cartCount})
        </button>
      )}
    </div>
  );
}
