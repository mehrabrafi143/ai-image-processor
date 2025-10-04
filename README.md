# Full-Stack Image Processing Application

## Overview

This project demonstrates an end-to-end full-stack application integrating a React frontend, a .NET backend API, and a Python-based AI service for image analysis. The system allows users to upload an image, processes it through an AI model hosted as a Flask web service, and displays the results on the frontend interface. The complete solution is designed to run both locally and on Microsoft Azure.

---

## 1. System Architecture

The solution is composed of three main components:

1. **Frontend (React JS)**  
   Provides the user interface where images can be uploaded and processed results are displayed.

2. **Backend (.NET API)**  
   Acts as a middleware layer that receives uploaded files from the frontend, communicates with the AI service, and returns processed results.

3. **AI Service (Python Flask)**  
   Implements image processing logic such as brightness and contrast detection, using OpenCV and Pillow.

Each service runs independently and communicates using REST APIs.

---

## 2. Local Setup Instructions

The following section explains how to run each part of the application locally.  
Ensure Node.js, .NET SDK, and Python are installed before starting.

### 2.1 AI Service (Python Flask)

**Address:** `http://localhost:5002`

1. Open a terminal in the Python AI project directory.  
2. Install dependencies using:
   ```bash
   pip install -r requirements.txt
   ```
3. Run the Flask service:
   ```bash
   python app.py
   ```
4. Verify the service by sending a test request:
   ```bash
   curl -X POST http://localhost:5002/process -F "image=@sample.png"
   ```

A successful response will return JSON output describing the image properties and classifications.

---

### 2.2 Backend (.NET API)

**Address:** `https://localhost:5001`

The backend receives images from the frontend and communicates with the AI service.

1. Navigate to the backend project folder.  
2. Build and start the server:
   ```bash
   dotnet build
   dotnet run
   ```
3. Confirm that the API is running at `https://localhost:5001`.

Ensure the `appsettings.json` includes the local AI service address:

```json
"AIService": {
  "Url": "http://localhost:5002/process"
}
```

When a request is received, the backend sends the image to the AI service and returns the AI-generated results to the frontend.

---

### 2.3 Frontend (React JS)

**Address:** `http://localhost:3000`

The frontend allows users to upload an image and view analysis results.

1. Navigate to the React project directory.  
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm start
   ```
4. The app will open automatically in the browser.  
   When an image is uploaded, it sends a POST request to:
   ```
   https://localhost:5001/api/process
   ```

---

### 2.4 Local Communication Flow

The data flow across the three components during local execution is as follows:

```
React Frontend (http://localhost:3000)
        ↓
.NET Backend API (https://localhost:5001)
        ↓
Python AI Service (http://localhost:5002)
```

Each component operates independently, allowing modular development and testing.

---

## 3. Deployment on Azure

The same architecture can be deployed seamlessly to Azure. Each component is hosted as a separate service while maintaining the same communication pattern.

### 3.1 AI Service Deployment (Python)

1. Package the Flask service as a container or deploy it to **Azure App Service**.  
2. Once deployed, note the new public endpoint, for example:
   ```
   https://your-ai-service.azurewebsites.net/process
   ```
3. Update the `.NET` backend configuration to use this Azure URL in `appsettings.json`:
   ```json
   "AIService": {
     "Url": "https://your-ai-service.azurewebsites.net/process"
   }
   ```

---

### 3.2 Backend Deployment (.NET API)

1. Publish the API to **Azure App Service** using Visual Studio or the `dotnet publish` command.  
2. After deployment, the backend will have a public URL such as:
   ```
   https://your-backend.azurewebsites.net/api/process
   ```
3. Verify that the deployed backend can successfully communicate with the AI service on Azure.

---

### 3.3 Frontend Deployment (React)

1. Update the React configuration so that the backend API URL points to the deployed Azure backend:
   ```
   REACT_APP_API_URL=https://your-backend.azurewebsites.net/api/process
   ```
2. Build the production version:
   ```bash
   npm run build
   ```
3. Deploy the build output to **Azure Static Web Apps** or an **Azure Storage Static Website**.  
   Example deployed URL:
   ```
   https://your-frontend.azurestaticapps.net
   ```

---

### 3.4 Azure Communication Flow

Once deployed, the system operates as follows:

```
React Frontend (Azure Static Web App)
        ↓
.NET Backend API (Azure App Service)
        ↓
Python AI Service (Azure App Service or Container)
```

Each layer communicates securely using HTTPS, maintaining the same logic and sequence as in the local setup.

---

## 4. Summary

| Environment | React App | .NET API | Python AI Service |
|--------------|------------|-----------|-------------------|
| **Local** | `http://localhost:3000` | `https://localhost:5001` | `http://localhost:5002` |
| **Azure** | `https://your-frontend.azurestaticapps.net` | `https://your-backend.azurewebsites.net` | `https://your-ai-service.azurewebsites.net` |

The system behaves consistently in both environments. The only configuration changes involve updating endpoint URLs in the relevant configuration files.

---

## 5. Evaluation Criteria Reference

The solution aligns with the provided technical assessment requirements:

- **Frontend:** Built in React, supports file uploads and displays AI-generated results.  
- **Backend:** Developed in .NET, handles authentication, image forwarding, and error handling.  
- **AI Service:** Implemented in Python using Flask and OpenCV, returns structured image analysis data.  
- **Integration:** All components communicate seamlessly using REST APIs.  
- **Deployment:** Fully deployable on Azure with clear setup instructions.