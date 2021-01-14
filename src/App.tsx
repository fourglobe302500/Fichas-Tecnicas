import { Box } from "@material-ui/core";
import React from "react";
import { Content } from "./components/Content";
import { Header } from "./components/Header";

function App() {
  return (
    <Box
      margin=".75rem"
      marginTop="1.5rem"
      padding=".75rem"
      border={2}
      borderColor="darkblue"
      borderRadius="3rem"
      color="cyan"
      minHeight="80vh"
      marginX="4rem"
    >
      <Header></Header>
      <Content>Hello this page is under development</Content>
    </Box>
  );
}

export default App;
