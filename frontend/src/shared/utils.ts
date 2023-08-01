import { Comment } from "@/api/dto/comment";
import { DataRow } from "@/shared/types";
import { DAYS, MONTHS, WEEKS } from "@/shared/consts";
import {
  startOfWeek,
  endOfWeek,
  subWeeks,
  isWithinInterval,
  formatISO,
} from "date-fns";

export const contributorsNumToString = (contributors_num: number): string => {
  if (contributors_num === 1) {
    return contributors_num + " contributor";
  }

  return contributors_num + " contributors";
};

export const timeToString = (time: number): string => {
  time = Math.floor(time / 60);
  let h = Math.floor(time / 60);
  let m = time % 60;

  if (m === 0) {
    return h + "h";
  }
  if (h === 0) {
    return m + "m";
  }

  return h + "h " + m + "m";
};

export const dateWithoutTime = (timestamp: string): string => {
  return timestamp.split("T")[0];
};

export const dateForTable = (date: string): string => {
  return date.split("-").reverse().join(".");
};

export const compareDates = (date1: string, date2: string): boolean => {
  const d1 = new Date(date1);
  const d2 = new Date(date2);

  return d2 >= d1;
};

const timeForPlot = (time: number): number => {
  return Math.floor(time / 60);
};

const getDaysArray = (start: string, end: string) => {
  let arr = [];
  for (
    let dt = new Date(start);
    dt <= new Date(end);
    dt.setDate(dt.getDate() + 1)
  ) {
    arr.push(dateWithoutTime(formatISO(new Date(dt))));
  }

  return arr;
};

export const divideIntoGroupsByDay = (comments: Comment[]): DataRow[] => {
  const today = new Date();
  const nDaysAgo = new Date(today.getTime() - (DAYS - 1) * 24 * 60 * 60 * 1000);

  const days = getDaysArray(
    dateWithoutTime(formatISO(nDaysAgo)),
    dateWithoutTime(formatISO(today))
  );

  let result = [] as DataRow[];
  for (let i = 0; i < days.length; i++) {
    result.push({ name: dateForTable(days[i]), issues: 0, mrs: 0 } as DataRow);

    for (let j = 0; j < comments.length; j++) {
      let comment = comments[j];

      if (dateWithoutTime(comment.created_at) === days[i]) {
        if (comment.type === "Issue") {
          result[result.length - 1].issues += timeForPlot(comment.time_spent);
        } else if (comment.type === "MR") {
          result[result.length - 1].mrs += timeForPlot(comment.time_spent);
        }
      }
    }
  }

  return result;
};

const getMonthsArray = (start: string, end: string) => {
  let startDate = start.split("-");
  let endDate = end.split("-");
  let startYear = parseInt(startDate[0]);
  let endYear = parseInt(endDate[0]);
  let months = [];

  for (let i = startYear; i <= endYear; i++) {
    let endMonth = i != endYear ? 11 : parseInt(endDate[1]) - 1;
    let startMon = i === startYear ? parseInt(startDate[1]) - 1 : 0;

    for (let j = startMon; j <= endMonth; j = j > 12 ? j % 12 || 11 : j + 1) {
      let month = j + 1;
      let displayMonth = month < 10 ? "0" + month : month;
      months.push([i, displayMonth].join("-"));
    }
  }

  return months;
};

const monthForPlot = (date: string): string => {
  let [y, m] = date.split("-");
  const mTyped = m as keyof typeof MONTHS;

  return MONTHS[mTyped] + " " + y;
};

export const divideIntoGroupsByMonth = (comments: Comment[]): DataRow[] => {
  const today = new Date();
  const yearAgo = new Date(today.getTime() - 365 * 24 * 60 * 60 * 1000);

  let months = getMonthsArray(
    dateWithoutTime(formatISO(yearAgo)),
    dateWithoutTime(formatISO(today))
  );
  months = months.slice(months.length - 12, months.length);

  let result = [] as DataRow[];
  for (let i = 0; i < months.length; i++) {
    result.push({
      name: monthForPlot(months[i]),
      issues: 0,
      mrs: 0,
    } as DataRow);

    for (let j = 0; j < comments.length; j++) {
      let comment = comments[j];

      if (dateWithoutTime(comment.created_at).slice(0, 7) === months[i]) {
        if (comment.type === "Issue") {
          result[result.length - 1].issues += timeForPlot(comment.time_spent);
        } else if (comment.type === "MR") {
          result[result.length - 1].mrs += timeForPlot(comment.time_spent);
        }
      }
    }
  }

  return result;
};

const getWeeksArray = (today: Date) => {
  let weeks = [];

  let startDay = startOfWeek(today, { weekStartsOn: 1 });
  let endDay = endOfWeek(today, { weekStartsOn: 1 });
  weeks.push({ start: startDay, end: endDay });

  for (let i = 1; i <= WEEKS + 1; i++) {
    let prevStartDay = startDay;
    let prevEndDay = endDay;

    startDay = subWeeks(prevStartDay, 1);
    endDay = subWeeks(prevEndDay, 1);

    weeks.push({ start: startDay, end: endDay });
  }

  return weeks.reverse();
};

const weekForPlot = (week: { start: Date; end: Date }): string => {
  return (
    dateForTable(dateWithoutTime(formatISO(week.start))) +
    "-" +
    dateForTable(dateWithoutTime(formatISO(week.end)))
  );
};

export const divideIntoGroupsByWeek = (comments: Comment[]): DataRow[] => {
  const today = new Date();

  let weeks = getWeeksArray(today);

  let result = [] as DataRow[];
  for (let i = 0; i < weeks.length; i++) {
    result.push({
      name: weekForPlot(weeks[i]),
      issues: 0,
      mrs: 0,
    } as DataRow);

    for (let j = 0; j < comments.length; j++) {
      let comment = comments[j];

      if (isWithinInterval(new Date(comment.created_at), weeks[i])) {
        if (comment.type === "Issue") {
          result[result.length - 1].issues += timeForPlot(comment.time_spent);
        } else if (comment.type === "MR") {
          result[result.length - 1].mrs += timeForPlot(comment.time_spent);
        }
      }
    }
  }

  return result;
};
