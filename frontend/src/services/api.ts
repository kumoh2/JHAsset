import axios from 'axios';

export const api = axios.create({
  baseURL: 'https://localhost:5001/api',   // 개발용 URL
});
