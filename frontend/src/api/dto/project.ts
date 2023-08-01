import { contributorsNumToString, timeToString } from "@/shared/utils";

export interface Project {
  id: number;
  title: string;
  contributors_num: number;
  issues_num: number;
  merge_requests_num: number;
  time_spent: number;
}

export interface ProjectForTable extends Record<string, string> {
  id: string;
  title: string;
  contributors_num: string;
  time_spent: string;
}

export const mapToProjectForTable = (project: Project) => {
  return {
    id: String(project.id),
    title: String(project.title),
    contributors_num: contributorsNumToString(project.contributors_num),
    time_spent: timeToString(project.time_spent),
  } as ProjectForTable;
};
