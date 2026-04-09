import { Routes, Route } from 'react-router-dom';
import { CartProvider } from './context/CartContext';
import { OrderProvider } from './context/OrderContext';
import MenuPage from './pages/MenuPage';
import CartPage from './pages/CartPage';
import CheckoutPage from './pages/CheckoutPage';
import OrderStatusPage from './pages/OrderStatusPage';

export default function App() {
  return (
    <CartProvider>
      <OrderProvider>
        <div className="app">
          <Routes>
            <Route path="/menu/:slug" element={<MenuPage />} />
            <Route path="/cart" element={<CartPage />} />
            <Route path="/checkout" element={<CheckoutPage />} />
            <Route path="/order/:orderId" element={<OrderStatusPage />} />
          </Routes>
        </div>
      </OrderProvider>
    </CartProvider>
  );
}
