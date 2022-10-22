import { http2 } from '../../http'

export const list = () => http2.get('/api/notice/notice')
export const edit = (data, edit) => {
  return http2[edit ? 'put' : 'post']('/api/notice/notice', data)
}
export const del = (id) => http2.delete(`/api/notice/notice?id=${id}`)
