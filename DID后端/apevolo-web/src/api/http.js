import axios from 'axios'

const http = axios.create({
  baseURL: 'http://192.168.2.110:5555',
  timeout: 10000
})

http.interceptors.request.use(
  config => {
    config.headers['Content-Type'] = 'application/json; charset=utf-8'
    return config
  },
  config => {
    return Promise.reject(config)
  }
)

http.interceptors.response.use(
  res => {
    if (res.data.code) {
      return Promise.reject(res.data)
    }
    return res.data
  },
  res => {
    return Promise.reject(res)
  }
)

export default http
