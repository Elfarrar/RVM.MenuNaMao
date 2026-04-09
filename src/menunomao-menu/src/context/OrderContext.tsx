import { createContext, useContext, useState, type ReactNode } from 'react';

interface OrderContextType {
  orderId: string | null;
  setOrderId: (id: string) => void;
}

const OrderContext = createContext<OrderContextType>(null!);

export function OrderProvider({ children }: { children: ReactNode }) {
  const [orderId, setOrderId] = useState<string | null>(null);

  return (
    <OrderContext.Provider value={{ orderId, setOrderId }}>
      {children}
    </OrderContext.Provider>
  );
}

export const useOrder = () => useContext(OrderContext);
