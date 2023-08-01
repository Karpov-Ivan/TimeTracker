import styles from "./Select.module.css";

type OptionValue = string | number;

interface SelectProps<Value extends OptionValue> {
  value: Value;
  label: string;
  options: readonly Value[];
  onChange: (newValue: Value) => void;
}

const Select = <Value extends OptionValue>({
  value,
  label,
  options,
  onChange,
}: SelectProps<Value>) => {
  return (
    <div className={styles.select__container}>
      {label && <label className={styles.select__label}>{label}</label>}
      <select
        className={styles.select}
        value={value}
        onChange={(event: React.FormEvent<HTMLSelectElement>) => {
          const selectedOption = options[event.currentTarget.selectedIndex];
          onChange(selectedOption);
        }}
      >
        {options.map((option) => (
          <option className={styles.option} key={option} value={option}>
            {option}
          </option>
        ))}
      </select>
    </div>
  );
};

export default Select;
