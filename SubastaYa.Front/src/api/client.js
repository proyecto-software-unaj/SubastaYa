const BASE_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5269";

let currentUserId = 2; 

export function setCurrentUserId(id) {
  currentUserId = id;
}

export function getCurrentUserId() {
  return currentUserId;
}


async function request(path, { method = "GET", body } = {}) {
  const options = {
    method,
    headers: {
      "Content-Type": "application/json",
      UserId: String(currentUserId),
    },
  };

  if (body !== undefined) {
    options.body = JSON.stringify(body);
  }

  const response = await fetch(`${BASE_URL}${path}`, options);

  
  if (response.status === 204) {
    return null;
  }

  
  const text = await response.text();
  const data = text ? JSON.parse(text) : null;

  if (!response.ok) {
    
    const message = data?.detail ?? data?.title ?? "Error en la solicitud.";
    const error = new Error(message);
    error.status = response.status;
    throw error;
  }

  return data;
}

export const api = {
  get: (path) => request(path),
  post: (path, body) => request(path, { method: "POST", body }),
};
