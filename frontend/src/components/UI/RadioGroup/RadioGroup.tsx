import styles from "./RadioGroup.module.css";

interface RadioGroupProps {
  options: string[];
  selectedOption: string;
  setSelectedOption: React.Dispatch<React.SetStateAction<string>>;
}

const RadioGroup = ({
  options,
  selectedOption,
  setSelectedOption,
}: RadioGroupProps) => {
  const handleOptionChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSelectedOption(event.target.value);
  };

  return (
    <div className={styles.radio_group}>
      {options.map((option) => (
        <label key={option} className={styles.radio_group__label}>
          <input
            className={styles.radio_group__input}
            type="radio"
            value={option}
            checked={selectedOption === option}
            onChange={handleOptionChange}
          />
          {option}
        </label>
      ))}
    </div>
  );
};

export default RadioGroup;
