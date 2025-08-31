import React from 'react';
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate,
} from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { useAuthStore } from './state/authStore';
import { Layout } from './components/Layout';
import { Login } from './pages/Login';
import { Dashboard } from './pages/Dashboard';

// Create a client
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      retry: 1,
      refetchOnWindowFocus: false,
    },
  },
});

// Protected Route Component
const ProtectedRoute: React.FC<{ children: React.ReactNode }> = ({
  children,
}) => {
  const { isAuthenticated } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to='/login' replace />;
  }

  return <>{children}</>;
};

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Routes>
          <Route path='/login' element={<Login />} />
          <Route
            path='/'
            element={
              <ProtectedRoute>
                <Layout>
                  <Dashboard />
                </Layout>
              </ProtectedRoute>
            }
          />
          <Route
            path='/projects'
            element={
              <ProtectedRoute>
                <Layout>
                  <div className='p-6'>
                    <h1 className='text-3xl font-bold text-gray-900'>
                      Projects
                    </h1>
                    <p className='mt-2 text-gray-600'>
                      Manage your projects here.
                    </p>
                  </div>
                </Layout>
              </ProtectedRoute>
            }
          />
          <Route
            path='/tasks'
            element={
              <ProtectedRoute>
                <Layout>
                  <div className='p-6'>
                    <h1 className='text-3xl font-bold text-gray-900'>Tasks</h1>
                    <p className='mt-2 text-gray-600'>
                      View and manage your tasks.
                    </p>
                  </div>
                </Layout>
              </ProtectedRoute>
            }
          />
          <Route
            path='/resources'
            element={
              <ProtectedRoute>
                <Layout>
                  <div className='p-6'>
                    <h1 className='text-3xl font-bold text-gray-900'>
                      Resources
                    </h1>
                    <p className='mt-2 text-gray-600'>
                      Book and manage resources.
                    </p>
                  </div>
                </Layout>
              </ProtectedRoute>
            }
          />
          <Route
            path='/users'
            element={
              <ProtectedRoute>
                <Layout>
                  <div className='p-6'>
                    <h1 className='text-3xl font-bold text-gray-900'>Users</h1>
                    <p className='mt-2 text-gray-600'>Manage team members.</p>
                  </div>
                </Layout>
              </ProtectedRoute>
            }
          />
          <Route
            path='/settings'
            element={
              <ProtectedRoute>
                <Layout>
                  <div className='p-6'>
                    <h1 className='text-3xl font-bold text-gray-900'>
                      Settings
                    </h1>
                    <p className='mt-2 text-gray-600'>
                      Configure your preferences.
                    </p>
                  </div>
                </Layout>
              </ProtectedRoute>
            }
          />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}

export default App;
