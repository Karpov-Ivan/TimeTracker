import { DataRow } from "@/shared/types";
import {
  BarChart,
  Bar,
  Legend,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";

interface HistogramProps {
  data: DataRow[];
}

const Histogram = ({ data }: HistogramProps) => {
  return (
    <ResponsiveContainer width="100%" height="100%">
      <BarChart data={data}>
        <CartesianGrid strokeDasharray="3 3" />
        <XAxis dataKey="name" style={{ fontSize: "14px" }} />
        <YAxis style={{ fontSize: "14px" }} />
        <Legend />
        <Tooltip />
        <Bar dataKey="issues" fill="#2CB05C" name="Issues" />
        <Bar dataKey="mrs" fill="#1994FC" name="Merge requests" />
      </BarChart>
    </ResponsiveContainer>
  );
};

export default Histogram;
