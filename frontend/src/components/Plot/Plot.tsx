import { useEffect, useState } from "react";
import { Comment } from "@/api/dto/comment";
import Modal from "@/components/UI/Modal";
import RadioGroup from "@/components/UI/RadioGroup";
import Histogram from "@/components/Histogram";
import { DataRow } from "@/shared/types";
import styles from "./Plot.module.css";
import {
  divideIntoGroupsByDay,
  divideIntoGroupsByWeek,
  divideIntoGroupsByMonth,
} from "@/shared/utils";

interface PlotProps {
  visible: boolean;
  setVisible: React.Dispatch<React.SetStateAction<boolean>>;
  comments: Comment[];
}

const Plot = ({ visible, setVisible, comments }: PlotProps) => {
  const options = ["по дням", "по неделям", "по месяцам"];

  const [selectedOption, setSelectedOption] = useState<string>("по дням");
  const [plotData, setPlotData] = useState<DataRow[]>([]);

  useEffect(() => {
    switch (selectedOption) {
      case "по месяцам":
        setPlotData(divideIntoGroupsByMonth(comments));
        break;
      case "по неделям":
        setPlotData(divideIntoGroupsByWeek(comments));
        break;
      case "по дням":
        setPlotData(divideIntoGroupsByDay(comments));
        break;
      default:
        break;
    }
  }, [selectedOption]);

  return (
    <Modal visible={visible} setVisible={setVisible}>
      <div className={styles.plot__container}>
        <div>
          <div>Построение графика</div>
          <div className={styles.plot__header}>
            <RadioGroup
              options={options}
              selectedOption={selectedOption}
              setSelectedOption={setSelectedOption}
            />
          </div>
        </div>
        <div className={styles.plot__sheet}>
          {plotData.length !== 0 && <Histogram data={plotData} />}
        </div>
      </div>
    </Modal>
  );
};

export default Plot;
