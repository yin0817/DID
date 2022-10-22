import { http2 } from '../../http'

export const list = () => http2.get('/api/volunteer/volunteer')
export const edit = (data, edit) => {
  return http2[edit ? 'put' : 'post']('/api/volunteer/volunteer', data)
}
export const del = (id) => http2.delete(`/api/volunteer/volunteer?id=${id}`)
export const upload = (data) => http2.post(`/api/destruction/uploadimage?type=volunteer`, data, { headers: { 'Content-Type': 'multipart/form-data' }})
