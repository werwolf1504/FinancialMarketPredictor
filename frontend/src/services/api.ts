import axios from "axios";

const api = axios.create({
    baseURL: 'https://localhost:7194',
    timeout: 10000,
    headers: {
        'Content-Type' : 'aplication/json',
    },
});

api.interceptors.response.use(
    (responce) => responce,
    (error) => {
        console.error('Api error: ', error.responce?.data || error.message);
        return Promise.reject(error);
    }
);

export default api;