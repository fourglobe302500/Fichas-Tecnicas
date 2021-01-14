import { Box } from "@material-ui/core";
import React from "react";

interface ContentProps {
  children: React.ReactNode;
}

export const Content: React.FC<ContentProps> = ({ children }) => {
  return (
    <Box
      display="flex"
      justifyContent="center"
      marginTop="1rem"
      fontSize="1.25rem"
    >
      {children}
    </Box>
  );
};
