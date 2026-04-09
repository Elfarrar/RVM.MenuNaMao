import { createContext, useContext, useState, type ReactNode } from 'react';

interface CartItem {
  menuItemId: string;
  name: string;
  price: number;
  quantity: number;
  notes: string;
}

interface CartContextType {
  items: CartItem[];
  tableToken: string;
  restaurantSlug: string;
  setTableToken: (token: string) => void;
  setRestaurantSlug: (slug: string) => void;
  addItem: (item: Omit<CartItem, 'quantity' | 'notes'>) => void;
  updateQuantity: (menuItemId: string, quantity: number) => void;
  updateNotes: (menuItemId: string, notes: string) => void;
  removeItem: (menuItemId: string) => void;
  clearCart: () => void;
  total: number;
}

const CartContext = createContext<CartContextType>(null!);

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartItem[]>([]);
  const [tableToken, setTableToken] = useState('');
  const [restaurantSlug, setRestaurantSlug] = useState('');

  const addItem = (item: Omit<CartItem, 'quantity' | 'notes'>) => {
    setItems(prev => {
      const existing = prev.find(i => i.menuItemId === item.menuItemId);
      if (existing) {
        return prev.map(i =>
          i.menuItemId === item.menuItemId ? { ...i, quantity: i.quantity + 1 } : i
        );
      }
      return [...prev, { ...item, quantity: 1, notes: '' }];
    });
  };

  const updateQuantity = (menuItemId: string, quantity: number) => {
    if (quantity <= 0) {
      removeItem(menuItemId);
      return;
    }
    setItems(prev => prev.map(i => (i.menuItemId === menuItemId ? { ...i, quantity } : i)));
  };

  const updateNotes = (menuItemId: string, notes: string) => {
    setItems(prev => prev.map(i => (i.menuItemId === menuItemId ? { ...i, notes } : i)));
  };

  const removeItem = (menuItemId: string) => {
    setItems(prev => prev.filter(i => i.menuItemId !== menuItemId));
  };

  const clearCart = () => setItems([]);

  const total = items.reduce((sum, i) => sum + i.price * i.quantity, 0);

  return (
    <CartContext.Provider
      value={{ items, tableToken, restaurantSlug, setTableToken, setRestaurantSlug, addItem, updateQuantity, updateNotes, removeItem, clearCart, total }}
    >
      {children}
    </CartContext.Provider>
  );
}

export const useCart = () => useContext(CartContext);
