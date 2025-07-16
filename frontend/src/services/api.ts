import axios from 'axios';

export const api = axios.create({
  baseURL: 'http://localhost:5000/api',   // 개발용 URL
  withCredentials: false                  // 쿠키 필요 없으면 false
});
