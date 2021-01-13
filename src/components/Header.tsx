import React from "react";
import "./Header.css";

interface HeaderProps {
  content: string;
}

export const Header: React.FC<HeaderProps> = ({ content }) => {
  return <div className="Header">{content}</div>;
};
