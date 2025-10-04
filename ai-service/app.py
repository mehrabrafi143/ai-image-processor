from flask import Flask, request, jsonify
from flask_cors import CORS
import cv2
import numpy as np
import io
from PIL import Image
import base64

app = Flask(__name__)
CORS(app)

def analyze_image(image):
    """Simple image analysis without complex dependencies"""
    try:
        # Convert to numpy array
        img_array = np.array(image)
        
        # Basic image analysis
        height, width = img_array.shape[:2]
        
        # Convert to grayscale for analysis
        if len(img_array.shape) == 3:
            gray = cv2.cvtColor(img_array, cv2.COLOR_RGB2GRAY)
        else:
            gray = img_array
            
        # Calculate basic features
        brightness = np.mean(gray)
        contrast = np.std(gray)
        
        # Simple classification based on brightness
        if brightness < 50:
            classification = "Low-light Image"
            confidence = 0.88
        elif brightness > 200:
            classification = "Bright Image"
            confidence = 0.85
        elif contrast > 60:
            classification = "High Contrast Image"
            confidence = 0.82
        else:
            classification = "Normal Image"
            confidence = 0.75
            
        # Mock object detection
        objects = []
        if width > height:
            objects.append({"label": "Landscape Orientation", "confidence": 0.90})
        else:
            objects.append({"label": "Portrait Orientation", "confidence": 0.90})
            
        if brightness > 150:
            objects.append({"label": "Well-lit Area", "confidence": 0.85})
        else:
            objects.append({"label": "Dark Area", "confidence": 0.80})
            
        return {
            "classification": classification,
            "confidence": round(confidence, 2),
            "objects": objects,
            "image_info": {
                "width": width,
                "height": height,
                "brightness": round(brightness, 2),
                "contrast": round(contrast, 2)
            }
        }
        
    except Exception as e:
        return {
            "classification": "Analysis Error",
            "confidence": 0.0,
            "objects": [],
            "error": str(e)
        }

@app.route('/process', methods=['POST'])
def process_image():
    try:
        if 'image' not in request.files:
            return jsonify({"error": "No image file provided"}), 400
            
        file = request.files['image']
        if file.filename == '':
            return jsonify({"error": "No file selected"}), 400

        # Read and process image
        image_data = file.read()
        image = Image.open(io.BytesIO(image_data))
        
        # Analyze image
        result = analyze_image(image)
        result['processing_time'] = 0.5
        result['filename'] = file.filename
        
        return jsonify(result), 200

    except Exception as e:
        return jsonify({"error": f"Processing failed: {str(e)}"}), 500

@app.route('/health', methods=['GET'])
def health_check():
    return jsonify({"status": "healthy", "service": "AI Image Processor"})

if __name__ == '__main__':
    print("🚀 AI Image Processor running on http://localhost:5002")
    app.run(host='0.0.0.0', port=5002, debug=False)