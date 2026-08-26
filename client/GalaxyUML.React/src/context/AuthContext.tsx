import React, { createContext, useContext, useEffect, useState } from 'react';
import { ApiService } from '../services/api';
import { User } from '../types';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (dto: { username: string; password: string }) => Promise<void>;
  register: (dto: {
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    password: string;
  }) => Promise<string>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => ApiService.getStoredUser());
  const [token, setToken] = useState<string | null>(() => ApiService.getToken());
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const storedToken = ApiService.getToken();
    const storedUser = ApiService.getStoredUser();
    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(storedUser);
    } else {
      setToken(null);
      setUser(null);
    }
    setIsLoading(false);
  }, []);

  const login = async (dto: { username: string; password: string }) => {
    const res = await ApiService.login(dto);
    setUser(res.user);
    setToken(res.token);
  };

  const register = async (dto: {
    firstName: string;
    lastName: string;
    username: string;
    email: string;
    password: string;
  }) => {
    return await ApiService.register(dto);
  };

  const logout = () => {
    ApiService.logout();
    setUser(null);
    setToken(null);
  };

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated: !!token && !!user,
        isLoading,
        login,
        register,
        logout,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
