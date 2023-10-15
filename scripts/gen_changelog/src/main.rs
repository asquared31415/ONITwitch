use core::fmt::Write;
use std::fs;
use std::path::Path;

use semver::Version;

fn process_path(root_path: &Path, path: &Path, depth: usize) -> String {
    let Ok(entries) = fs::read_dir(path) else {
        panic!("Could not read dir {}", path.display())
    };

    // Ignore erroring entries and entries for which a file type cannot be determined.
    let (mut dirs, files) = entries
        .filter_map(|e| e.ok())
        .filter(|e| e.file_type().is_ok())
        .partition::<Vec<_>, _>(|entry| entry.file_type().unwrap().is_dir());

    // Ignore files that cannot parse as semver
    let mut sorted_files = files
        .into_iter()
        .filter_map(|entry| {
            // Files should always have a stem
            Version::parse(&entry.path().file_stem().unwrap().to_string_lossy())
                .ok()
                .map(|v| (entry, v))
        })
        .collect::<Vec<_>>();
    // Sort the files by their semver
    sorted_files.sort_by(|(_, v1), (_, v2)| v1.cmp(&v2).reverse());

    // The output string to write to the file
    let mut s = format!("{}items:\n", "  ".repeat(depth));

    for (entry, _version) in sorted_files {
        let path = entry.path();
        let name = path.file_stem().unwrap().to_string_lossy();
        let _ = writeln!(s, "{}- name: {}", "  ".repeat(depth + 1), name);
        let _ = writeln!(
            s,
            "{}href: {}",
            "  ".repeat(depth + 2),
            path.strip_prefix(root_path).unwrap().display()
        );
    }

    // Sort dirs by path
    dirs.sort_by_key(|e| e.path());

    // Write dir entries
    for dir in dirs {
        let name = dir.file_name();
        let name = name.to_string_lossy();
        let _ = writeln!(s, "{}- name: {}", "  ".repeat(depth + 1), name);
        // Depth +1 from the name, +1 for the traversal
        let dir_str = process_path(root_path, &dir.path(), depth + 2);
        s.push_str(dir_str.as_str());
    }

    s
}

fn main() {
    let args = std::env::args().collect::<Vec<_>>();
    let [_, path] = args.as_slice() else {
        panic!("Specify the changelogs directory")
    };
    let path = Path::new(path);
    let Ok(true) = path.try_exists() else {
        panic!("Path `{}` could not be verified to exist", path.display())
    };
    if !path.is_dir() {
        panic!("Path `{}` was not a directory", path.display())
    }

    let final_toc = process_path(path, path, 0);
    fs::write(path.join("toc.yml"), final_toc).unwrap();
}
