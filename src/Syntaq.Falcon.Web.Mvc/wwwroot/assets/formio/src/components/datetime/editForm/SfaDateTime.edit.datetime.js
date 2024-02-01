export default [
  {
    type: 'heading',
    input: false,
    heading: 'Date',
    weight: 0
  },
  {
    type: 'checkbox',
    input: true,
    key: 'enableDate',
    label: 'Enable Date Input',
    weight: 10,
    tooltip: 'Enables date input for this field.'
  },
  {
    type: 'textfield',
    input: true,
    key: 'datePicker.minDate',
    label: 'Minimum Date',
    placeholder: 'yyyy-MM-dd',
    tooltip: 'The minimum date that can be picked. You can also use Moment.js functions. For example: \n \n moment().subtract(10, \'days\')',
    weight: 20
  },
  {
    type: 'textfield',
    input: true,
    key: 'datePicker.maxDate',
    label: 'Maximum Date',
    placeholder: 'yyyy-MM-dd',
    tooltip: 'The maximum date that can be picked. You can also use Moment.js functions. For example: \n \n moment().add(10, \'days\')',
    weight: 30
  },
  {
    type: 'heading',
    input: false,
    heading: 'Time',
    weight: 40
  },
  {
    type: 'checkbox',
    input: true,
    key: 'enableTime',
    label: 'Enable Time Input',
    tooltip: 'Enables time input for this field.',
    weight: 50
  },
  {
    type: 'number',
    input: true,
    key: 'timePicker.hourStep',
    label: 'Hour Step Size',
    tooltip: 'The number of hours to increment/decrement in the time picker.',
    weight: 60
  },
  {
    type: 'number',
    input: true,
    key: 'timePicker.minuteStep',
    label: 'Minute Step Size',
    tooltip: 'The number of minutes to increment/decrement in the time picker.',
    weight: 70
  },
  {
    type: 'checkbox',
    input: true,
    key: 'timePicker.showMeridian',
    label: '12 Hour Time (AM/PM)',
    tooltip: 'Display time in 12 hour time with AM/PM.',
    weight: 80
  }
];
