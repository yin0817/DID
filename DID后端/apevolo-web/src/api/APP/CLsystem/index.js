import { http2 } from '../../http'

export const list = () => http2.get('/api/clsystem/clsystem')
export const edit = (data, edit) => {
  return http2[edit ? 'put' : 'post']('/api/clsystem/clsystem', data)
}
export const del = (id) => http2.delete(`/api/clsystem/clsystem?id=${id}`)
