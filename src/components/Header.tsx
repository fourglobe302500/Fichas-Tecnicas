import { Box } from "@material-ui/core";
import React from "react";

interface HeaderProps {
  content?: string;
}

export const Header: React.FC<HeaderProps> = ({ content = "Main Page" }) => {
  return (
    <Box
      display="flex"
      justifyContent="center"
      fontSize="2rem"
      marginX="auto"
      marginTop="1rem"
      maxWidth="fit-content"
      padding="1rem"
      border={1}
      borderColor="gray"
      borderRadius="borderRadius"
    >
      {content}
    </Box>
  );
};
