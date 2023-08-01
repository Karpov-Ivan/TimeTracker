import Header from "@/components/Header";
import Input from "@/components/UI/Input";
import Table from "@/components/UI/Table";
import { useMemo, useState, useEffect } from "react";
import { Project, mapToProjectForTable } from "@/api/dto/project";
import projectService from "@/api/projectService";
import { PROJECTS_ROUTE } from "@/shared/consts";
import { useUserContext } from "@/context/UserContext";
import styles from "./PageProjects.module.css";

const PageProjects = () => {
  const { user } = useUserContext();

  const [currentPage, setCurrentPage] = useState<number>(1);
  const [fetching, setFetching] = useState<boolean>(true);
  const [totalCount, setTotalCount] = useState<number>(0);

  const hasScroll = document.body.scrollHeight > document.body.clientHeight;

  const [projects, setProjects] = useState<Project[]>([]);
  const [filter, setFilter] = useState({ query: "" });

  const searchedProjects = useMemo(() => {
    return projects.filter((project) =>
      project.title.toLocaleLowerCase().includes(filter.query.toLowerCase())
    );
  }, [filter.query, projects]);

  useEffect(() => {
    if (fetching) {
      projectService
        .getAll(currentPage, user?.token || "")
        .then(
          (result) => {
            if (result?.status === 200) {
              setProjects([...projects, ...result.data.projects]);
              setCurrentPage((prevState) => prevState + 1);
              if (result.data.total_count !== -1)
                setTotalCount(result.data.total_count);
            }
          },
          (error) => console.log(error)
        )
        .finally(() => setFetching(false));
    }
  }, [fetching]);

  useEffect(() => {
    const scrollHandler = (e: any) => {
      if (
        e.target.documentElement.scrollHeight -
          (e.target.documentElement.scrollTop + window.innerHeight) <
          100 &&
        projects.length < totalCount
      ) {
        setFetching(true);
      }
    };

    document.addEventListener("scroll", scrollHandler);
    return function () {
      document.removeEventListener("scroll", scrollHandler);
    };
  }, [totalCount, projects]);

  return (
    <div>
      <Header title={"TimeTracker"} />
      <div className={styles.page_projects__container}>
        <div className={styles.page_projects__content_header}>
          <div className={styles.page_projects__content_title}>Проекты</div>
          <Input
            id="projects"
            name="projects"
            type="text"
            placeholder="Поиск проектов"
            value={filter.query}
            onChange={(e) => setFilter({ ...filter, query: e.target.value })}
          />
        </div>
        <div>
          <Table
            data={searchedProjects.map((project) =>
              mapToProjectForTable(project)
            )}
            columns={[
              { label: "Название", attribute: "title", width: "75%" },
              {
                label: "Количество участников",
                attribute: "contributors_num",
                width: "15%",
              },
              { label: "Время", attribute: "time_spent", width: "10%" },
            ]}
            ifHeader={false}
            ifID={false}
            pathToRowPage={PROJECTS_ROUTE}
          />
          {fetching && (
            <div className={styles.page_projects__content_loading}>
              Загрузка...
            </div>
          )}
          {!hasScroll && !fetching && projects.length < totalCount && (
            <div
              className={styles.page_projects__content_loader}
              onClick={() => setFetching(true)}
            >
              Загрузить еще
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default PageProjects;
