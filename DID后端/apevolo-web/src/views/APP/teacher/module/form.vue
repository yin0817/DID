<template>
  <el-dialog
    append-to-body
    :title="isEdit ? '编辑' : '添加'"
    :visible.sync='show'
    :close-on-click-modal="false"
    :before-close='reset'
    width="500px"
  >
    <el-form
      ref="form"
      :model="form.data"
      :rules="form.rules"
      size="small"
      label-width="80px"
    >
      <el-form-item
        label="教师名称"
        prop="name"
      >
        <el-input
          v-model="form.data.name"
          placeholder='请输入教师名称'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="教师头像"
        prop="imageUrl"
      >
        <el-upload
          class="avatar-uploader"
          action=""
          list-type='image/jpeg,image/png'
          :show-file-list="false"
          :before-upload="beforeAvatarUpload"
          :http-request='onUpload'
          :on-success='handleAvatarSuccess'
          :on-error='onErrorUpload'
        >
          <img v-if="form.data.headImage" :src="transformSrc(form.data.headImage)" class="avatar">
          <i v-else class="el-icon-plus avatar-uploader-icon"></i>
        </el-upload>
      </el-form-item>
      <el-form-item
        label="教师简介"
        prop="blurb"
      >
        <el-input
          v-model="form.data.blurb"
          clearable
          :rows='6'
          resize='none'
          type='textarea'
          placeholder='请输入教师简介'
        />
      </el-form-item>
    </el-form>
    <div
      slot="footer"
      class="dialog-footer"
    >
      <el-button
        type="text"
        @click="toggle(false)"
      >
        取消
      </el-button>
      <el-button
        :loading="form.loading"
        :disabled="form.loading"
        type="primary"
        @click="submit"
      >
        确认
      </el-button>
    </div>
  </el-dialog>
</template>

<script>
import { edit, upload } from '@/api/APP/teacher'
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
          name: '',
          blurb: '',
          headImage: ''
        },
        rules: {
          name: [{ required: true, message: '请输入教师名称', trigger: 'blur' }],
          blurb: [{ required: true, message: '请输入简介', trigger: 'blur' }],
          headImage: [{ required: true, message: '请上传教师头像', trigger: 'blur' }]
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
    // 重置
    reset(done) {
      this.$refs.form.resetFields()
      this.isEdit = false
      done && done()
    },
    beforeAvatarUpload(file) {
      return file.type.includes('image/')
    },
    async onUpload({ file }) {
      const formData = new FormData()
      formData.append('file', file)
      return await upload(formData)
    },
    handleAvatarSuccess(res, file) {
      this.form.data.headImage = res.message
    },
    onErrorUpload() {
      this.$message.error('图片上传失败，请重新上传！')
      this.form.data.headImage = ''
    },
    submit() {
      this.$refs.form.validate(valid => {
        if (valid) {
          this.form.loading = true
          edit(this.form.data, this.isEdit).then(() => {
            this.$message.success(`${this.isEdit ? '编辑' : '添加'}成功！`)
            this.toggle(false)
            this.reset()
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

<style rel="stylesheet/scss" lang="scss" scoped>
 ::v-deep {
   .el-input-number .el-input__inner {
     text-align: left;
   }
   .ql-container {
     height: 300px;
   }
 }
</style>
<style lang='scss'>
.avatar-uploader .el-upload {
  border: 1px dashed #d9d9d9;
  border-radius: 6px;
  cursor: pointer;
  position: relative;
  overflow: hidden;
}
.avatar-uploader .el-upload:hover {
  border-color: #409EFF;
}
.avatar-uploader-icon {
  font-size: 28px;
  color: #8c939d;
  width: 178px;
  height: 178px;
  line-height: 178px;
  text-align: center;
}
.avatar {
  width: 178px;
  height: 178px;
  display: block;
}
</style>
