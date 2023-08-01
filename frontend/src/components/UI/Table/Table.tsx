import { useNavigate } from "react-router-dom";
import styles from "./Table.module.css";

interface TableRowProps {
  row: Record<string, string>;
  rowIndex: number;
  columns: TableColumn[];
  columnWidths: string[];
  ifID: boolean;
  pathToRowPage: string;
}

const TableRow = ({
  row,
  rowIndex,
  columns,
  columnWidths,
  ifID,
  pathToRowPage,
}: TableRowProps) => {
  const navigate = useNavigate();
  const cursorStyle = pathToRowPage ? "pointer" : "normal";

  return (
    <tr
      key={rowIndex}
      onClick={() =>
        pathToRowPage && navigate(pathToRowPage + "/" + String(row.id))
      }
      style={{ cursor: cursorStyle }}
    >
      {columns.map(
        (column, columnIndex) =>
          (ifID || column.attribute !== "id") && (
            <td key={columnIndex} style={{ width: columnWidths[columnIndex] }}>
              {row[column.attribute]}
            </td>
          )
      )}
    </tr>
  );
};

interface TableColumn {
  label: string;
  attribute: string;
  width: string;
}

interface TableProps<T> {
  columns: TableColumn[];
  data: T[];
  ifHeader: boolean;
  ifID: boolean;
  pathToRowPage: string;
}

const Table = <T extends Record<string, string>>({
  columns,
  data,
  ifHeader,
  ifID,
  pathToRowPage,
}: TableProps<T>) => {
  const columnWidths = columns.map((column) => column.width);

  return (
    <div className={styles.table__container}>
      <table className={styles.table}>
        {ifHeader && (
          <thead>
            <tr>
              {columns.map((column, index) => (
                <th key={index} style={{ width: columnWidths[index] }}>
                  {column.label}
                </th>
              ))}
            </tr>
          </thead>
        )}
        <tbody>
          {data.map((row, index) => (
            <TableRow
              key={index}
              row={row}
              rowIndex={index}
              columns={columns}
              columnWidths={columnWidths}
              ifID={ifID}
              pathToRowPage={pathToRowPage}
            />
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default Table;
