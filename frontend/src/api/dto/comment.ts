import { timeToString, dateWithoutTime, dateForTable } from "@/shared/utils";

export interface Comment {
  id: number;
  project_id: number;
  author: string;
  title: string;
  created_at: string;
  time_spent: number;
  type: string;
}

export interface CommentForTable extends Record<string, string> {
  id: string;
  title: string;
  author: string;
  created_at: string;
  time_spent: string;
  type: string;
}

export const mapToCommentForTable = (comment: Comment) => {
  return {
    id: String(comment.id),
    title: String(comment.title),
    author: String(comment.author),
    created_at: dateForTable(dateWithoutTime(comment.created_at)),
    time_spent: timeToString(comment.time_spent),
    type: String(comment.type),
  } as CommentForTable;
};
