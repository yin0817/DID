import { http2 } from '../../http'

export const list = () => http2.get('/api/order/order')
export const edit = (data, edit) => {
  return http2[edit ? 'put' : 'post']('/api/order/order', data)
}
export const del = (id) => http2.delete(`/api/order/order?id=${id}`)
