import { getTokens, setTokens, clearTokens } from '../utils/token';
  
let isRefreshing = false;
let refreshSubscribers: ((token: string) => void)[] = [];

const subscribeTokenRefresh = (cb: (token: string) => void) => {
  refreshSubscribers.push(cb);
};

const onRefreshed = (newToken: string) => {
  refreshSubscribers.forEach((cb) => cb(newToken));
  refreshSubscribers = [];
};

export async function apiClient(endpoint: string, options: RequestInit = {}): Promise<Response> {
  const baseUrl = import.meta.env.VITE_API_URL || 'https://localhost:7068';
  const { token, refreshToken } = getTokens();

  const headers = new Headers(options.headers);
  if (!headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json');
  }

  if (token) {
    headers.set('Authorization', `Bearer ${token}`);
  }

  const config: RequestInit = {
    ...options,
    headers,
  };

  let response = await fetch(`${baseUrl}${endpoint}`, config);

  if (response.status === 401) {
    if (!token || !refreshToken) {
      clearTokens();
      window.location.href = '/login';
      throw new Error('Authentication required.');
    }

    if (!isRefreshing) {
      isRefreshing = true;

      try {
        const refreshResponse = await fetch(`${baseUrl}/api/auth/refresh`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            jwtToken: token,
            refreshToken: refreshToken,
          }),
        });

        if (!refreshResponse.ok) {
          throw new Error('Session expired');
        }

        const data = await refreshResponse.json();
        
        setTokens(data.token, data.refreshToken);
        isRefreshing = false;
        
        onRefreshed(data.token);

        headers.set('Authorization', `Bearer ${data.token}`);
        response = await fetch(`${baseUrl}${endpoint}`, { ...config, headers });
        return response;

      } catch (error) {
        isRefreshing = false;
        refreshSubscribers = []; 
        clearTokens();
        window.location.href = '/login'; 
        throw error;
      }
    } else {
      return new Promise((resolve) => {
        subscribeTokenRefresh(async (newToken: string) => {
          headers.set('Authorization', `Bearer ${newToken}`);
          const retryResponse = await fetch(`${baseUrl}${endpoint}`, { ...config, headers });
          resolve(retryResponse);
        });
      });
    }
  }

  return response;
}