import React from 'react';
import ReactDOM from 'react-dom/client';
import SearchComponent from './SearchComponent';

// Перевірка наявності елемента з ID 'search-component'
const rootElement = document.getElementById('search-component');

if (rootElement) {
    // Використання createRoot для рендерингу компонента
    const root = ReactDOM.createRoot(rootElement);
    root.render(<SearchComponent />);
} else {
    console.error("Не знайдено елемент з ID 'search-component'.");
}
