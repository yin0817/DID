import axios from 'axios'
import Cookies from 'js-cookie'

function requestInterceptorsFilled(config) {
  config.headers['Content-Type'] = 'application/json;'
  config.headers['Authorization'] = Cookies.get('APEVOLO-TOEKN')
  return config
}
function requestInterceptorsRejected(config) {
  return Promise.reject(config)
}

function responseInterceptorsFilled(res) {
  if (res.data.code) {
    return Promise.reject(res.data)
  }
  return res.data
}
function responseInterceptorsRejected(res) {
  return Promise.reject(res)
}

const http1 = axios.create({
  baseURL: 'http://192.168.2.110:5555',
  timeout: 10000
})
http1.interceptors.request.use(requestInterceptorsFilled, requestInterceptorsRejected)
http1.interceptors.response.use(responseInterceptorsFilled, responseInterceptorsRejected)

const http2 = axios.create({
  baseURL: process.env.VUE_APP_BASE_API2,
  timeout: 10000
})
http2.interceptors.request.use(requestInterceptorsFilled, requestInterceptorsRejected)
http2.interceptors.response.use(responseInterceptorsFilled, responseInterceptorsRejected)

export { http1, http2 }
