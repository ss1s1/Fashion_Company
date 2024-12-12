import React, { useState, useEffect } from 'react';

function SearchComponent() {
    const [query, setQuery] = useState('');
    const [results, setResults] = useState([]);

    // Обновление результатов при каждом изменении query
    useEffect(() => {
        if (query.trim() === '') {
            setResults([]); // Если строка поиска пуста, очистим результаты
        } else {
            // Если строка поиска не пуста, отправим запрос на сервер
            fetch(`/Catalog/Search?query=${encodeURIComponent(query)}`)
                .then((response) => response.json())
                .then((data) => setResults(data))
                .catch((error) => console.error('Ошибка:', error));
        }
    }, [query]);

    const handleInputChange = (e) => {
        setQuery(e.target.value);
    };

    return (
        <div className="search-container mb-3">
            <input
                type="text"
                name="query"
                className="form-control"
                placeholder="Начните вводить для поиска"
                value={query}
                onChange={handleInputChange}
                style={{
                    backgroundColor: '#f8f9fa',
                    border: '1px solid #ced4da',
                    textAlign: 'center',
                    color: '#6c757d',
                }}
            />

            <div className="row mt-3">
                {results.length > 0 ? (
                    results.map((item) => (
                        <div key={item.id} className="col-lg-4 col-md-6 col-sm-12">
                            <div className="card">
                                <div className="card-inner">
                                    {/* Передняя сторона */}
                                    <div className="card-front">
                                        <img src={item.imageUrl} alt={item.name} className="card-img-top" />
                                        <div className="card-body">
                                            <h5 className="card-title">{item.name}</h5>
                                            <div className={item.editor ? "info-row" : "info-centered"}>
                                                <p><strong>Author:</strong> {item.author}</p>
                                                {item.editor && <p style={{ fontSize: '0.8rem' }}><strong>Last Editor:</strong> {item.editor}</p>}
                                            </div>
                                            <p><strong>Modification:</strong> {new Date(item.createdDate).toLocaleString()}</p>
                                            <button className="btn btn-outline-primary">Детальнее</button>
                                        </div>
                                    </div>
                                    {/* Задняя сторона */}
                                    <div className="card-back">
                                        <h5 className="card-title">{item.name}</h5>
                                        <div className="card-body">
                                            <p>{item.description}</p>
                                            <button className="btn btn-outline-primary">К фото</button>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))
                ) : query.trim() === '' ? (
                    <p> </p>
                ) : (
                    <p className="text-center text-muted">Ничего не найдено</p>
                )}
            </div>
        </div>
    );
}

export default SearchComponent;

