import axios from 'axios';

export const api = axios.create({
  baseURL: 'https://localhost:7107',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true,
});
