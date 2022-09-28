import http from '../http'

export const list = () => http.post('/api/destruction/getdestruction', {}, { 'Content-type': 'application/json' })
export const add = (data) => http.post('/api/destruction/adddestruction', data, { 'Content-type': 'application/json' })
export const del = (id) => http.post(`/api/destruction/destruction?destructionId=${id}`, { 'Content-type': 'application/json' })

export default { add, del, list }
