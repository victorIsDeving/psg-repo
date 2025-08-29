cmake_minimum_required(VERSION 3.16)

# Set C++ standard
set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Project name
project(VanGogh)

# Include directories
include_directories(${PROJECT_SOURCE_DIR}/includes)

# Add executable
add_executable(VanGogh stb_image/stb_image.cpp glad.c VanGogh.cpp)

# Find and link GLFW
find_package(glfw3 REQUIRED)
target_link_libraries(VanGogh glfw)

# Find and link GLEW
find_package(GLEW REQUIRED)
target_link_libraries(VanGogh GLEW::GLEW)

# macOS-specific configurations
if (APPLE)
    # Find and link macOS frameworks
    find_library(COCOA_LIBRARY Cocoa)
    find_library(OPENGL_LIBRARY OpenGL)
    find_library(GLUT_LIBRARY GLUT)
    target_link_libraries(VanGogh ${COCOA_LIBRARY} ${OPENGL_LIBRARY} ${GLUT_LIBRARY})
else()
    # For non-macOS systems, link GL and dl
    target_link_libraries(VanGogh GL dl)
endif()