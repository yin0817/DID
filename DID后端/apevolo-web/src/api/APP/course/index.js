import { http2 } from '../../http'

export const list = () => http2.get('/api/course/course')
export const edit = (data, edit) => {
  return http2[edit ? 'put' : 'post']('/api/course/course', data)
}
export const del = (id) => http2.delete(`/api/course/course?id=${id}`)
export const upload = (data) => http2.post(`/api/destruction/uploadimage?type=course`, data, { headers: { 'Content-Type': 'multipart/form-data' }})
