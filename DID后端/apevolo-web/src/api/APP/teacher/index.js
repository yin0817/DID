import { http2 } from '../../http'

export const list = () => http2.get('/api/teacher/teacher')
export const edit = (data, edit) => {
  return http2[edit ? 'put' : 'post']('/api/teacher/teacher', data)
}
export const del = (id) => http2.delete(`/api/teacher/teacher?id=${id}`)
export const upload = (data) => http2.post(`/api/destruction/uploadimage?type=APPAvatar`, data, { headers: { 'Content-Type': 'multipart/form-data' }})
