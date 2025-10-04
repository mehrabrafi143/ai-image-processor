import React, { useState } from 'react';
import './App.css';

function App() {
  const [selectedFile, setSelectedFile] = useState(null);
  const [previewUrl, setPreviewUrl] = useState(null);
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleFileSelect = (event) => {
    const file = event.target.files[0];
    if (file && file.type.startsWith('image/')) {
      setSelectedFile(file);
      setPreviewUrl(URL.createObjectURL(file));
      setResult(null);
      setError('');
    }
  };

  const handleUpload = async () => {
    if (!selectedFile) return;

    setLoading(true);
    setError('');

    const formData = new FormData();
    formData.append('image', selectedFile);

    try {
      const response = await fetch('https://localhost:5001/api/process', {
        method: 'POST',
        headers: {
          'Authorization': 'Bearer ' + process.env.REACT_APP_API_KEY
        },
        body: formData
      });

      if (!response.ok) {
        throw new Error('Processing failed');
      }

      const data = await response.json();
      setResult(data);
    } catch (err) {
      setError('Failed to process image. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="app">
      <header className="app-header">
        <h1>AI Image Processor</h1>
        <p>Upload an image for AI-powered analysis</p>
      </header>

      <main className="main-content">
        <div className="upload-section">
          <div className="file-input-container">
            <input
              type="file"
              accept="image/*"
              onChange={handleFileSelect}
              className="file-input"
              id="file-input"
            />
            <label htmlFor="file-input" className="file-input-label">
              Choose Image
            </label>
            {selectedFile && (
              <span className="file-name">{selectedFile.name}</span>
            )}
          </div>

          {previewUrl && (
            <div className="preview-section">
              <div className="image-container">
                <img src={previewUrl} alt="Preview" className="preview-image" />
              </div>
              <button 
                onClick={handleUpload} 
                disabled={loading}
                className="process-button"
              >
                {loading ? 'Processing...' : 'Process Image'}
              </button>
            </div>
          )}

          {error && (
            <div className="error-message">
              {error}
            </div>
          )}

          {result && (
            <div className="result-section">
              <h2>Analysis Results</h2>
              <div className="result-card">
                <h3>Classification: {result.classification}</h3>
                <p>Confidence: {(result.confidence * 100).toFixed(2)}%</p>
                {result.objects && result.objects.length > 0 && (
                  <div>
                    <h4>Detected Objects:</h4>
                    <ul>
                      {result.objects.map((obj, index) => (
                        <li key={index}>
                          {obj.label} ({(obj.confidence * 100).toFixed(1)}%)
                        </li>
                      ))}
                    </ul>
                  </div>
                )}
                <p className="processing-time">
                  Processed in {result.processing_time}s
                </p>
              </div>
            </div>
          )}
        </div>
      </main>
    </div>
  );
}

export default App;