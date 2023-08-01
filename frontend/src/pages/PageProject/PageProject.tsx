import { useEffect, useState, useMemo } from "react";
import { useParams } from "react-router-dom";
import Header from "@/components/Header";
import Button from "@/components/UI/Button";
import Input from "@/components/UI/Input";
import Select from "@/components/UI/Select";
import Table from "@/components/UI/Table";
import { compareDates, timeToString, dateWithoutTime } from "@/shared/utils";
import { Project } from "@/api/dto/project";
import { Comment, mapToCommentForTable } from "@/api/dto/comment";
import projectService from "@/api/projectService";
import Plot from "@/components/Plot";
import { useUserContext } from "@/context/UserContext";
import styles from "./PageProject.module.css";

const PageProject = () => {
  const { id } = useParams();
  const { user } = useUserContext();

  const [project, setProject] = useState<Project>();
  const [comments, setComments] = useState<Comment[]>([]);

  const initialFilterValues = {
    id: "",
    type: "",
    author: "",
    period_start: "",
    period_end: "",
  };
  const [filter, setFilter] = useState(initialFilterValues);

  const filteredComments = useMemo(() => {
    return comments.filter(
      (comment) =>
        (comment.id === Number(filter.id) ||
          filter.id === initialFilterValues.id) &&
        (comment.author === filter.author ||
          filter.author === initialFilterValues.author) &&
        (comment.type === filter.type ||
          filter.type === initialFilterValues.type) &&
        (compareDates(
          filter.period_start,
          dateWithoutTime(comment.created_at)
        ) ||
          filter.period_start === initialFilterValues.period_start) &&
        (compareDates(dateWithoutTime(comment.created_at), filter.period_end) ||
          filter.period_end === initialFilterValues.period_end)
    );
  }, [filter, comments]);

  const types = ["", "MR", "Issue"];
  const [authors, setAuthors] = useState<string[]>([]);

  const [visible, setVisible] = useState<boolean>(false);

  useEffect(() => {
    projectService.getComments(Number(id) || -1, user?.token || "").then(
      (result) => {
        if (result?.status === 200) {
          const newComments = result.data;
          setComments(newComments);
          setAuthors([
            "",
            ...new Set(
              newComments.map(({ author }: { author: string }) => author)
            ),
          ] as string[]);
        }
      },
      (error) => console.log(error)
    );
  }, []);

  useEffect(() => {
    projectService.get(Number(id) || -1, user?.token || "").then(
      (result) => {
        if (result?.status === 200) {
          setProject(result.data);
        }
      },
      (error) => console.log(error)
    );
  }, []);

  const timeSpent = (types: string[]) => {
    let result = 0;
    filteredComments.forEach((comment) => {
      if (types.includes(comment.type)) {
        result += comment.time_spent;
      }
    });

    return result;
  };

  return (
    <div>
      <Header title={project?.title || ""} />
      <div className={styles.page_project__container}>
        <div className={styles.page_project__summary}>
          <div>
            <div>Всего: {timeToString(timeSpent(["MR", "Issue"]))}</div>
            <div>Merge requests: {timeToString(timeSpent(["MR"]))}</div>
            <div>Issues: {timeToString(timeSpent(["Issue"]))}</div>
          </div>
          <div>
            <Button onClick={() => setVisible(true)}>Построить график</Button>
          </div>
        </div>
        <div className={styles.page_project__filters}>
          <Input
            id="id_filter"
            name="id_filter"
            type="number"
            label="ID"
            min="1"
            value={filter.id}
            onChange={(e) => setFilter({ ...filter, id: e.target.value })}
          />
          <Select
            label="Тип"
            value={filter.type}
            options={types}
            onChange={(e) => setFilter({ ...filter, type: e })}
          />
          <Select
            label="Автор"
            value={filter.author}
            options={authors}
            onChange={(e) => setFilter({ ...filter, author: e })}
          />
          <Input
            id="period_start_filter"
            name="period_start_filter"
            type="date"
            label="Начало периода"
            value={filter.period_start}
            onChange={(e) =>
              setFilter({ ...filter, period_start: e.target.value })
            }
          />
          <Input
            id="period_end_filter"
            name="period_end_filter"
            type="date"
            label="Конец периода"
            value={filter.period_end}
            onChange={(e) =>
              setFilter({ ...filter, period_end: e.target.value })
            }
          />
          <div>
            <Button
              onClick={() => {
                setFilter(initialFilterValues);
              }}
            >
              Очистить
            </Button>
          </div>
        </div>
        <div className={styles.page_project__table}>
          <Table
            columns={[
              { label: "ID", attribute: "id", width: "5%" },
              { label: "Название", attribute: "title", width: "40%" },
              { label: "Автор", attribute: "author", width: "20%" },
              { label: "Дата", attribute: "created_at", width: "15%" },
              { label: "Время", attribute: "time_spent", width: "10%" },
              {
                label: "Тип",
                attribute: "type",
                width: "5%",
              },
            ]}
            data={filteredComments.map((comment) =>
              mapToCommentForTable(comment)
            )}
            ifHeader={true}
            ifID={true}
            pathToRowPage=""
          />
        </div>
      </div>
      {visible && (
        <Plot
          visible={visible}
          setVisible={setVisible}
          comments={filteredComments}
        />
      )}
    </div>
  );
};

export default PageProject;
