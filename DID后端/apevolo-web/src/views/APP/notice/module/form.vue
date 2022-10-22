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
        label="公告标题"
        prop="title"
      >
        <el-input
          v-model="form.data.title"
          placeholder='请输入公告标题'
          clearable
        />
      </el-form-item>
      <el-form-item
        label="公告内容"
        prop="content"
      >
        <quill-editor
          v-model='form.data.content'
          ref='quillEditor'
          :options='{}'
        ></quill-editor>
      </el-form-item>
      <el-form-item
        label="创建人"
        prop="creatorName"
      >
        <el-input
          v-model="form.data.creatorName"
          placeholder='请输入创建人'
          clearable
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
import { edit } from '@/api/APP/notice'

import { quillEditor } from 'vue-quill-editor'
import 'quill/dist/quill.core.css'
import 'quill/dist/quill.snow.css'
import 'quill/dist/quill.bubble.css'

export default {
  data() {
    return {
      show: false,
      isEdit: false,
      title: '',
      form: {
        loading: false,
        data: {
          title: '',
          content: '',
          creatorName: 'EOTC技术团队'
        },
        rules: {
          title: [{ required: true, message: '请输入公告标题', trigger: 'blur' }],
          content: [{ required: true, message: '请输入公告内容', trigger: 'blur' }],
          creatorName: [{ required: true, message: '请输入发布团队', trigger: 'blur' }]
        }
      }
    }
  },
  components: {
    quillEditor
  },
  methods: {
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
