<template>
  <el-dialog
    append-to-body
    :title="isEdit ? '编辑' : '添加'"
    :visible.sync='show'
    :close-on-click-modal='false'
    :before-close='reset'
    width='500px'
  >
    <el-form
      ref='form'
      :model='form.data'
      :rules='form.rules'
      size='small'
      label-width='140px'
    >
      <el-form-item
        label='志愿者微信号'
        prop='wechat'
      >
        <el-input
          v-model='form.data.wechat'
          placeholder='请输入志愿者微信号'
          clearable
        />
      </el-form-item>
      <el-form-item
        label='志愿者微信二维码'
        prop='qrCode'
      >
        <el-upload
          action=''
          list-type='picture-card'
          :show-file-list="false"
          :before-upload='beforeUpload'
          :http-request='onUpload'
          :on-success='handleSuccess'
          :on-error='onErrorUpload'
        >
          <img v-if='form.data.qrCode' style='display: block; width: 100%;' :src='transformSrc(form.data.qrCode)' class='avatar'>
          <i v-else class='el-icon-plus avatar-uploader-icon'></i>
        </el-upload>
      </el-form-item>
    </el-form>
    <div
      slot='footer'
      class='dialog-footer'
    >
      <el-button
        type='text'
        @click='toggle(false)'
      >
        取消
      </el-button>
      <el-button
        :loading='form.loading'
        :disabled='form.loading'
        type='primary'
        @click='submit'
      >
        确认
      </el-button>
    </div>
  </el-dialog>
</template>
<script>
import { edit, upload } from '@/api/APP/volunteer'
import { transformSrc } from '@/utils'

export default {
  data() {
    return {
      show: false,
      isEdit: false,
      title: '',
      form: {
        loading: false,
        data: {
          wechat: '',
          qrCode: ''
        },
        rules: {
          wechat: [{ required: true, message: '请输入志愿者微信号', trigger: 'blur' }],
          qrCode: [{ required: true, message: '请上传志愿者微信二维码', trigger: 'blur' }]
        }
      }
    }
  },
  methods: {
    transformSrc,
    toggle(show, data) {
      !show && this.reset()
      this.isEdit = !!data
      this.show = show
      !!data && this.$nextTick().then(() => {
        Object.assign(this.form.data, data)
      })
    },
    beforeUpload(file) {
      const isIMG = file.type.includes('image/')
      const isLt4M = file.size / 1024 / 1024 < 4

      if (!isIMG) {
        this.$message.error('二维码只能是图片!')
      }
      if (!isLt4M) {
        this.$message.error('上传二维码图片大小不能超过 4MB!')
      }
      return isIMG && isLt4M
    },
    async onUpload({ file }) {
      const formData = new FormData()
      formData.append('file', file)
      return await upload(formData)
    },
    handleSuccess(res) {
      this.form.data.qrCode = res.message
    },
    onErrorUpload() {
      this.$message.error('图片上传失败，请重新上传！')
    },
    // 重置
    reset(done) {
      this.$refs.form.resetFields()
      this.isEdit = false
      done && done()
    },
    submit() {
      this.$refs.form.validate(valid => {
        if (valid) {
          this.form.loading = true
          edit(this.form.data, this.isEdit).then(() => {
            this.$message.success(`${this.isEdit ? '编辑' : '添加'}成功！`)
            this.toggle(false)
            this.$emit('refresh')
          }).catch(() => {
            this.$message.error(`${this.isEdit ? '编辑' : '添加'}失败！`)
          }).finally(() => {
            this.form.loading = false
          })
        }
      })
    }
  }
}
</script>
<style rel='stylesheet/scss' lang='scss' scoped>
::v-deep {
  .el-input-number .el-input__inner {
    text-align: left;
  }

  .ql-container {
    height: 300px;
  }
}
</style>
