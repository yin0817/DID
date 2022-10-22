import { http1 } from '../../http'

export const list = () => http1.post('/api/destruction/getdestruction', {}, { 'Content-type': 'application/json' })
export const add = (data) => http1.post('/api/destruction/adddestruction', data, { 'Content-type': 'application/json' })
export const del = (id) => http1.post(`/api/destruction/destruction?destructionId=${id}`, { 'Content-type': 'application/json' })

export default { add, del, list }
