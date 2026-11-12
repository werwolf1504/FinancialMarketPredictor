import { Dashboard } from './layouts/Dashboard';

function App() {
  return (
    <div className="min-h-screen bg-slate-50">
      {/* Верхня панель навігації */}
      <header className="bg-white shadow-sm border-b border-slate-200 px-8 py-4">
        <h1 className="text-2xl font-bold text-blue-600">
          Financial Platform
        </h1>
      </header>

      {/* Основний контент сторінки */}
      <main className="max-w-7xl mx-auto px-8 py-8">
        <Dashboard />
      </main>
    </div>
  );
}

export default App;