export default {
  label: 'Children',
  key: 'children',
  type: 'sectionz',
  input: true,
  components: [
    {
      type: 'panel',
      label: 'User Information',
      key: 'userinfo',
      components: [
        {
          label: 'First Name',
          key: 'firstName',
          type: 'textfield',
          input: true
        },
        {
          label: 'Last Name',
          key: 'lastName',
          type: 'textfield',
          input: true
        }
      ]
    }
  ]
};
