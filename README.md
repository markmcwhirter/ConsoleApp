
var sanitizedValue = value.replace(/<script[^>]*>([\s\S]*?)<\/script>/gi, "")
                              .replace(/["'><]/g, "");
